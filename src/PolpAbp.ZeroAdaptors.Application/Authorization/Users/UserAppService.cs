using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Localization;
using PolpAbp.Framework.Identity;
using PolpAbp.ZeroAdaptors.Authorization.Accounts;
using PolpAbp.ZeroAdaptors.Authorization.Accounts.Dto;
using PolpAbp.ZeroAdaptors.Authorization.Permissions;
using PolpAbp.ZeroAdaptors.Authorization.Permissions.Dto;
using PolpAbp.ZeroAdaptors.Authorization.Users.Dto;
using PolpAbp.ZeroAdaptors.Organizations.Dto;
using PolpAbp.ZeroAdaptors.Security;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Volo.Abp;
using Volo.Abp.Identity;

namespace PolpAbp.ZeroAdaptors.Authorization.Users
{
    [RemoteService(false)]
    public class UserAppService : ZeroAdaptorsAppService, IUserAppService
    {

        private readonly IIdentityRoleRepository _identityRoleRepository;
        private readonly IOrganizationUnitRepository _organizationUnitRepository;
        private readonly IdentityUserManager _identityUserManager;
        private readonly IIdentityUserRepository _identityUserRepository;
        private readonly IAccountAppService _accountAppService;
        private readonly IRegisteredUserDataSeeder _registeredUserDataSeeder;
        private readonly ISystemPermissionAppService _systemPermissionAppService;
        private readonly IUserPermissionAppService _userPermissionAppService;
        private readonly IStringLocalizerFactory _stringLocalizerFactory;

        public UserAppService(IIdentityRoleRepository identityRoleRepository,
            IOrganizationUnitRepository organizationUnitRepository,
            IdentityUserManager identityUserManager,
            IIdentityUserRepository identityUserRepository,
            IAccountAppService accountAppService,
            IRegisteredUserDataSeeder registeredUserDataSeeder,
            ISystemPermissionAppService systemPermissionAppService,
            IUserPermissionAppService userPermissionAppService,
            IStringLocalizerFactory stringLocalizerFactory
            )
        {
            _identityRoleRepository = identityRoleRepository;
            _organizationUnitRepository = organizationUnitRepository;
            _identityUserManager = identityUserManager;
            _identityUserRepository = identityUserRepository;
            _accountAppService = accountAppService;
            _registeredUserDataSeeder = registeredUserDataSeeder;
            _systemPermissionAppService = systemPermissionAppService;
            _userPermissionAppService = userPermissionAppService;
            _stringLocalizerFactory = stringLocalizerFactory;
        }

        [Authorize(IdentityPermissions.Users.Create)]
        public async Task<GetUserForEditOutput> GetUserForEditAsync()
        {
            var roles = await _identityRoleRepository.GetListAsync();
            var userRoleDtos = roles.OrderBy(x => x.NormalizedName)
                .Select(r => new UserRoleDto
                {
                    RoleId = r.Id,
                    RoleName = r.Name,
                    RoleDisplayName = r.NormalizedName
                })
                .ToArray();

            var allOrganizationUnits = await _organizationUnitRepository.GetListAsync();
            var ouDtos = allOrganizationUnits
                .Select(x => ObjectMapper.Map<OrganizationUnit, OrganizationUnitDto>(x))
                .ToList();

            var output = new GetUserForEditOutput
            {
                Roles = userRoleDtos,
                AllOrganizationUnits = ouDtos,
                MemberedOrganizationUnits = new List<string>()
            };

            output.User = new UserEditDto
            {
                IsActive = true,
                ShouldChangePasswordOnNextLogin = true,
                IsTwoFactorEnabled = false, // todo: Read from setting manager
                IsLockoutEnabled = false // todo: Read from setting manager
            };

            foreach (var defaultRole in roles.Where(r => r.IsDefault).ToList())
            {
                var defaultUserRole = userRoleDtos.FirstOrDefault(ur => ur.RoleName == defaultRole.Name);
                if (defaultUserRole != null)
                {
                    defaultUserRole.IsAssigned = true;
                }
            }

            return output;
        }

        [Authorize(IdentityPermissions.Users.Update)]
        public async Task<GetUserForEditOutput> GetUserForEditAsync(Guid input)
        {
            var roles = await _identityRoleRepository.GetListAsync();
            var userRoleDtos = roles.OrderBy(x => x.NormalizedName)
                .Select(r => new UserRoleDto
                {
                    RoleId = r.Id,
                    RoleName = r.Name,
                    RoleDisplayName = r.NormalizedName
                })
                .ToArray();

            var allOrganizationUnits = await _organizationUnitRepository.GetListAsync();
            var ouDtos = allOrganizationUnits
                .Select(x => ObjectMapper.Map<OrganizationUnit, OrganizationUnitDto>(x))
                .ToList();

            var output = new GetUserForEditOutput
            {
                Roles = userRoleDtos,
                AllOrganizationUnits = ouDtos,
                MemberedOrganizationUnits = new List<string>()
            };

            //Editing an existing user
            var user = await _identityUserManager.GetByIdAsync(input);

            output.User = new UserEditDto
            {
                Id = user.Id,
                Email = user.Email,
                EmailAddress = user.Email, // Client expect email address
                Name = user.Name,
                Surname = user.Surname,
                UserName = user.UserName,
                PhoneNumber = user.PhoneNumber
                // todo:
                // isactive
                // ....
            };

            var organizationUnits = await _identityUserManager.GetOrganizationUnitsAsync(user);
            output.MemberedOrganizationUnits = organizationUnits.Select(ou => ou.Code).ToList();

            var allRolesOfUsersOrganizationUnits = new List<string>();
            foreach (var o in organizationUnits)
            {
                var rs = await _identityUserRepository.GetRoleNamesInOrganizationUnitAsync(o.Id);
                allRolesOfUsersOrganizationUnits.AddRange(rs);
            }

            foreach (var userRoleDto in userRoleDtos)
            {
                userRoleDto.IsAssigned = await _identityUserManager.IsInRoleAsync(user, userRoleDto.RoleName);
                userRoleDto.InheritedFromOrganizationUnit = allRolesOfUsersOrganizationUnits.Contains(userRoleDto.RoleName);
            }
            return output;
        }


        [Authorize(IdentityPermissions.Users.Create)]
        public async Task<Guid> CreateUserAsync(CreateOrUpdateUserInput input)
        {
            // todo: Check max user restriction 


            // Normalize values so that we can leverage the helper 
            // method to set up roles and other. 
            input.User.Email = input.User.EmailAddress;
            input.User.RoleNames = input.AssignedRoleNames;
            //Set password
            if (input.SetRandomPassword)
            {
                var randomPassword = CreateRandomPassword();
                // Generated password should conform to our restrication
                input.User.Password = randomPassword;
            }
            else if (!input.User.Password.IsNullOrEmpty())
            {
                // todo: Validate the password???
            }

            var user = new IdentityUser(
               GuidGenerator.Create(),
               input.User.UserName,
               input.User.EmailAddress,
               CurrentTenant.Id
           );

            (await _identityUserManager.CreateAsync(user, input.User.Password)).CheckErrors();
            await UpdateUserByInput(user, input.User);

            await CurrentUnitOfWork.SaveChangesAsync(); // Next make sense to send notifications 

            // Add into org.
            await _identityUserManager.SetOrganizationUnitsAsync(user, input.OrganizationUnits.ToArray());

            // todo: Any seed data ????
            // Next seed data for this user, e.g., user role and so on.
            await _registeredUserDataSeeder.SeedAsync(input.User.EmailAddress, CurrentTenant.Id);

            // Emailing 
            if (input.SendActivationEmail)
            {
                await _accountAppService.SendEmailActivationLink(new SendEmailActivationLinkInput
                {
                    EmailAddress = input.User.Email
                });
            }

            return user.Id;
        }

        [Authorize(IdentityPermissions.Users.Update)]
        public async Task UpdateUserAsync(CreateOrUpdateUserInput input)
        {
            // Normalize values so that we can leverage the helper 
            // method to set up roles and other. 
            input.User.Email = input.User.EmailAddress;
            input.User.RoleNames = input.AssignedRoleNames;

            var user = await _identityUserManager.GetByIdAsync(input.User.Id.Value);
            // todo: ??
            // user.ConcurrencyStamp = input.ConcurrencyStamp;

            (await _identityUserManager.SetUserNameAsync(user, input.User.UserName)).CheckErrors();

            await UpdateUserByInput(user, input.User);
            // input.MapExtraPropertiesTo(user);

            (await _identityUserManager.UpdateAsync(user)).CheckErrors();

            //Set password
            if (input.SetRandomPassword)
            {
                var randomPassword = CreateRandomPassword();
                input.User.Password = randomPassword;
            }
            else
            {
                // Run validation ????
            }
            
            if (!input.User.Password.IsNullOrEmpty())
            {
                // todo: Validate the password???
                (await _identityUserManager.RemovePasswordAsync(user)).CheckErrors();
                (await _identityUserManager.AddPasswordAsync(user, input.User.Password)).CheckErrors();
            }

            await _identityUserManager.SetOrganizationUnitsAsync(user, input.OrganizationUnits.ToArray());

            await CurrentUnitOfWork.SaveChangesAsync();

            // Emailing 
            if (input.SendActivationEmail)
            {
                await _accountAppService.SendEmailActivationLink(new SendEmailActivationLinkInput
                {
                    EmailAddress = input.User.Email
                });
            }
        }

        [Authorize(IdentityPermissions.Users.ManagePermissions)]
        public async Task<GetUserPermissionsForEditOutput> GetUserPermissionsForEditAsync(Guid input)
        {
            var user = await _identityUserManager.GetByIdAsync(input);

            var permDefs = await _systemPermissionAppService.GetAllPermissionsAsync(true /* excluding host */);
            var permissions = permDefs.Select(x => new FlatPermissionDto
            {
                Name = x.Name,
                ParentName = x.Parent?.Name,
                DisplayName = x.DisplayName.Localize(_stringLocalizerFactory)
            }).ToList();

            var permNames = permDefs.Select(x => x.Name).ToList();
            var grantedDefs = await _userPermissionAppService.GetGrantedPermissionsAsync(user, permNames);
            var grantPermissions = grantedDefs.Select(x => x).ToList();
                
            return new GetUserPermissionsForEditOutput
            {
                Permissions = permissions,
                GrantedPermissionNames = grantPermissions
            };
        }

        [Authorize(IdentityPermissions.Users.ManagePermissions)]
        public async Task UpdateUserPermissionsAsync(UpdateUserPermissionsInput input)
        {
            var user = await _identityUserManager.GetByIdAsync(input.Id);
            var normlizedPermissions = input.GrantedPermissionNames
                .Select(x => x).ToList();

            await _userPermissionAppService.SetGrantedPermissionsAsync(user, normlizedPermissions);
        }

        [Authorize(IdentityPermissions.Users.ManagePermissions)]
        public async Task ResetUserSpecificPermissionsAsync(Guid id)
        {
            var user = await _identityUserManager.GetByIdAsync(id);
            var normlizedPermissions = new List<string>();

            await _userPermissionAppService.SetGrantedPermissionsAsync(user, normlizedPermissions);
        }


        protected string CreateRandomPassword()
        {
            // todo: Read from settings
            var passwordComplexitySetting = new PasswordComplexitySetting();

            var upperCaseLetters = "ABCDEFGHJKLMNOPQRSTUVWXYZ";
            var lowerCaseLetters = "abcdefghijkmnopqrstuvwxyz";
            var digits = "0123456789";
            var nonAlphanumerics = "!@$?_-";

            string[] randomChars = {
                upperCaseLetters,
                lowerCaseLetters,
                digits,
                nonAlphanumerics
            };

            var rand = new Random(Environment.TickCount);
            var chars = new List<char>();

            if (passwordComplexitySetting.RequireUppercase)
            {
                chars.Insert(rand.Next(0, chars.Count),
                    upperCaseLetters[rand.Next(0, upperCaseLetters.Length)]);
            }

            if (passwordComplexitySetting.RequireLowercase)
            {
                chars.Insert(rand.Next(0, chars.Count),
                    lowerCaseLetters[rand.Next(0, lowerCaseLetters.Length)]);
            }

            if (passwordComplexitySetting.RequireDigit)
            {
                chars.Insert(rand.Next(0, chars.Count),
                    digits[rand.Next(0, digits.Length)]);
            }

            if (passwordComplexitySetting.RequireNonAlphanumeric)
            {
                chars.Insert(rand.Next(0, chars.Count),
                    nonAlphanumerics[rand.Next(0, nonAlphanumerics.Length)]);
            }

            for (var i = chars.Count; i < passwordComplexitySetting.RequiredLength; i++)
            {
                var rcs = randomChars[rand.Next(0, randomChars.Length)];
                chars.Insert(rand.Next(0, chars.Count),
                    rcs[rand.Next(0, rcs.Length)]);
            }

            return new string(chars.ToArray());
        }

        protected virtual async Task UpdateUserByInput(IdentityUser user, UserEditDto input)
        {
            if (!string.Equals(user.Email, input.Email, StringComparison.InvariantCultureIgnoreCase))
            {
                (await _identityUserManager.SetEmailAsync(user, input.Email)).CheckErrors();
            }

            if (!string.Equals(user.PhoneNumber, input.PhoneNumber, StringComparison.InvariantCultureIgnoreCase))
            {
                (await _identityUserManager.SetPhoneNumberAsync(user, input.PhoneNumber)).CheckErrors();
            }

            (await _identityUserManager.SetLockoutEnabledAsync(user, input.LockoutEnabled)).CheckErrors();

            user.Name = input.Name;
            user.Surname = input.Surname;

            if (input.RoleNames != null)
            {
                (await _identityUserManager.SetRolesAsync(user, input.RoleNames)).CheckErrors();
            }
        }
    }
}
