using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Localization;
using PolpAbp.Framework.Identity;
using PolpAbp.ZeroAdaptors.Authorization.Accounts;
using PolpAbp.ZeroAdaptors.Authorization.Accounts.Dto;
using PolpAbp.ZeroAdaptors.Authorization.Permissions;
using PolpAbp.ZeroAdaptors.Authorization.Permissions.Dto;
using PolpAbp.ZeroAdaptors.Authorization.Users.Dto;
using PolpAbp.ZeroAdaptors.Authorization.Users.Events;
using PolpAbp.ZeroAdaptors.Organizations.Dto;
using PolpAbp.ZeroAdaptors.Security;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Volo.Abp;
using Volo.Abp.Data;
using Volo.Abp.EventBus.Local;
using Volo.Abp.Identity;

namespace PolpAbp.ZeroAdaptors.Authorization.Users
{
    [RemoteService(false)]
    public class UserAppService : ZeroAdaptorsAppService, IUserAppService
    {

        protected readonly IIdentityRoleRepository IdentityRoleRepository;
        protected readonly IOrganizationUnitRepository OrganizationUnitRepository;
        protected readonly IdentityUserManager IdentityUserManager;
        protected readonly IIdentityUserRepository IdentityUserRepository;
        protected readonly IAccountAppService AccountAppService;
        protected readonly IRegisteredUserDataSeeder RegisteredUserDataSeeder;
        protected readonly ISystemPermissionAppService SystemPermissionAppService;
        protected readonly IUserPermissionAppService UserPermissionAppService;
        protected readonly ILocalEventBus LocalEventBus;
        protected readonly IUserIdentityAssistantAppService UserIdentityAssistantAppService;

        public UserAppService(IIdentityRoleRepository identityRoleRepository,
            IOrganizationUnitRepository organizationUnitRepository,
            IdentityUserManager identityUserManager,
            IIdentityUserRepository identityUserRepository,
            IAccountAppService accountAppService,
            IRegisteredUserDataSeeder registeredUserDataSeeder,
            ISystemPermissionAppService systemPermissionAppService,
            IUserPermissionAppService userPermissionAppService,
            ILocalEventBus localEventBus,
            IUserIdentityAssistantAppService userIdentityAssistantAppService
            )
        {
            IdentityRoleRepository = identityRoleRepository;
            OrganizationUnitRepository = organizationUnitRepository;
            IdentityUserManager = identityUserManager;
            IdentityUserRepository = identityUserRepository;
            AccountAppService = accountAppService;
            RegisteredUserDataSeeder = registeredUserDataSeeder;
            SystemPermissionAppService = systemPermissionAppService;
            UserPermissionAppService = userPermissionAppService;
            LocalEventBus = localEventBus;
            UserIdentityAssistantAppService = userIdentityAssistantAppService;
        }

        [Authorize(IdentityPermissions.Users.Create)]
        public async Task<GetUserForEditOutput> GetUserForCreateAsync()
        {
            var roles = await IdentityRoleRepository.GetListAsync();
            var userRoleDtos = roles.OrderBy(x => x.NormalizedName)
                .Select(r => new UserRoleDto
                {
                    RoleId = r.Id,
                    RoleName = r.Name,
                    RoleDisplayName = r.NormalizedName
                })
                .ToArray();

            var allOrganizationUnits = await OrganizationUnitRepository.GetListAsync();
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
            var roles = await IdentityRoleRepository.GetListAsync();
            var userRoleDtos = roles.OrderBy(x => x.NormalizedName)
                .Select(r => new UserRoleDto
                {
                    RoleId = r.Id,
                    RoleName = r.Name,
                    RoleDisplayName = r.NormalizedName
                })
                .ToArray();

            var allOrganizationUnits = await OrganizationUnitRepository.GetListAsync();
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
            var user = await IdentityUserManager.GetByIdAsync(input);

            output.User = new UserEditDto
            {
                Id = user.Id,
                Email = user.Email,
                EmailAddress = user.Email, // Client expect email address
                Name = user.Name,
                Surname = user.Surname,
                UserName = user.UserName,
                PhoneNumber = user.PhoneNumber,
                IsTwoFactorEnabled = user.TwoFactorEnabled,
                IsLockoutEnabled = user.LockoutEnabled,
                ShouldChangePasswordOnNextLogin = user.GetProperty<bool>(nameof(UserEditDto.ShouldChangePasswordOnNextLogin), false)
                // todo:
                // isactive
                // ....
            };

            var organizationUnits = await IdentityUserManager.GetOrganizationUnitsAsync(user);
            output.MemberedOrganizationUnits = organizationUnits.Select(ou => ou.Code).ToList();

            var allRolesOfUsersOrganizationUnits = new List<string>();
            foreach (var o in organizationUnits)
            {
                var rs = await IdentityUserRepository.GetRoleNamesInOrganizationUnitAsync(o.Id);
                allRolesOfUsersOrganizationUnits.AddRange(rs);
            }

            foreach (var userRoleDto in userRoleDtos)
            {
                userRoleDto.IsAssigned = await IdentityUserManager.IsInRoleAsync(user, userRoleDto.RoleName);
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
                var validatedRet = await UserIdentityAssistantAppService.ValidatePasswordAsync(input.User.Password);
                if (!validatedRet.Succeeded)
                {
                    throw new AbpIdentityResultException(validatedRet);
                }
            }

            var user = new IdentityUser(
               GuidGenerator.Create(),
               input.User.UserName,
               input.User.EmailAddress,
               CurrentTenant.Id
           );

            var changedEvent = new ProfileChangedEvent();
            (await IdentityUserManager.CreateAsync(user, input.User.Password)).CheckErrors();
            await UpdateUserByInput(user, input.User, changedEvent);

            await CurrentUnitOfWork.SaveChangesAsync(); // Next make sense to send notifications 

            // Add into org.
            await IdentityUserManager.SetOrganizationUnitsAsync(user, input.OrganizationUnits.ToArray());

            // todo: Any seed data ????
            // Next seed data for this user, e.g., user role and so on.
            await RegisteredUserDataSeeder.SeedAsync(input.User.EmailAddress, CurrentTenant.Id);

            // Emailing 
            if (input.SendActivationEmail)
            {
                await AccountAppService.SendEmailActivationLink(new SendEmailActivationLinkInput
                {
                    EmailAddress = input.User.Email
                });
            }

            // Should change password on next login
            var passwordChangedEvent = new PasswordChangedEvent()
            {
                UserId = user.Id,
                OperatorId = CurrentUser?.Id,
                TenantId = CurrentUser.TenantId,
                NewPassword = input.User.Password
            };

            await LocalEventBus.PublishAsync(passwordChangedEvent);

            return user.Id;
        }

        [Authorize(IdentityPermissions.Users.Update)]
        public async Task UpdateUserAsync(CreateOrUpdateUserInput input)
        {
            var changedEvent = new ProfileChangedEvent()
            {
                OperatorId = CurrentUser?.Id
            };

            // Normalize values so that we can leverage the helper 
            // method to set up roles and other. 
            input.User.Email = input.User.EmailAddress;
            input.User.RoleNames = input.AssignedRoleNames;

            var user = await IdentityUserManager.GetByIdAsync(input.User.Id.Value);
            // todo: ??
            // user.ConcurrencyStamp = input.ConcurrencyStamp;
            await UpdateUserByInput(user, input.User, changedEvent);
            // todo: 
            // input.MapExtraPropertiesTo(user);

            (await IdentityUserManager.UpdateAsync(user)).CheckErrors();

            //Set password
            if (input.SetRandomPassword)
            {
                var randomPassword = CreateRandomPassword();
                input.User.Password = randomPassword;
            }
            else if (!input.User.Password.IsNullOrEmpty())
            {
                // todo: Validate the password???
                var validatedRet = await UserIdentityAssistantAppService.ValidatePasswordAsync(input.User.Password);
                if (!validatedRet.Succeeded)
                {
                    throw new AbpIdentityResultException(validatedRet);
                }
            }

            PasswordChangedEvent passwordChangedEvent = null;

            if (!input.User.Password.IsNullOrEmpty())
            {
                // The following will validate the password.
                // todo: Validate the password???
                (await IdentityUserManager.RemovePasswordAsync(user)).CheckErrors();
                (await IdentityUserManager.AddPasswordAsync(user, input.User.Password)).CheckErrors();

                passwordChangedEvent = new PasswordChangedEvent()
                {
                    UserId = user.Id,
                    OperatorId = CurrentUser?.Id,
                    TenantId = CurrentUser.TenantId,
                    NewPassword = input.User.Password
                };
            }

            await IdentityUserManager.SetOrganizationUnitsAsync(user, input.OrganizationUnits.ToArray());

            await CurrentUnitOfWork.SaveChangesAsync();

            // Emailing 
            if (input.SendActivationEmail)
            {
                await AccountAppService.SendEmailActivationLink(new SendEmailActivationLinkInput
                {
                    EmailAddress = input.User.Email
                });
            }

            // Events 
            if (passwordChangedEvent != null)
            {
                await LocalEventBus.PublishAsync(passwordChangedEvent);
            }
            await LocalEventBus.PublishAsync(changedEvent);
        }

        public async Task ResetUserPasswordAsync(Guid id, ResetUserPasswordDto input, bool runValidator)
        {
            var user = await IdentityUserManager.GetByIdAsync(id);

            //Set password
            if (input.SetRandomPassword)
            {
                var randomPassword = CreateRandomPassword();
                input.Password = randomPassword;
            }
            else if (runValidator)
            {
                // todo: Validate the password???
                var validatedRet = await UserIdentityAssistantAppService.ValidatePasswordAsync(input.Password);
                if (!validatedRet.Succeeded)
                {
                    throw new AbpIdentityResultException(validatedRet);
                }
            }

                // The following will validate the password.
                // todo: Validate the password???
                (await IdentityUserManager.RemovePasswordAsync(user)).CheckErrors();
            (await IdentityUserManager.AddPasswordAsync(user, input.Password)).CheckErrors();

            var passwordChangedEvent = new PasswordChangedEvent()
            {
                UserId = user.Id,
                OperatorId = CurrentUser?.Id,
                TenantId = CurrentUser.TenantId,
                NewPassword = input.Password
            };

            await LocalEventBus.PublishAsync(passwordChangedEvent);
        }

        [Authorize(IdentityPermissions.Users.ManagePermissions)]
        public async Task<GetUserPermissionsForEditOutput> GetUserPermissionsForEditAsync(Guid input)
        {
            var user = await IdentityUserManager.GetByIdAsync(input);

            var permDefs = await SystemPermissionAppService.GetAllPermissionsAsync(true /* excluding host */);
            var permissions = permDefs.Select(x => new FlatPermissionDto
            {
                Name = x.Name,
                ParentName = x.Parent?.Name,
                DisplayName = x.DisplayName.Localize(StringLocalizerFactory)
            }).ToList();

            var permNames = permDefs.Select(x => x.Name).ToList();
            var grantedDefs = await UserPermissionAppService.GetGrantedPermissionsAsync(user, permNames);
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
            var user = await IdentityUserManager.GetByIdAsync(input.Id);
            var normlizedPermissions = input.GrantedPermissionNames
                .Select(x => x).ToList();

            await UserPermissionAppService.SetGrantedPermissionsAsync(user, normlizedPermissions);
        }

        [Authorize(IdentityPermissions.Users.ManagePermissions)]
        public async Task ResetUserSpecificPermissionsAsync(Guid id)
        {
            var user = await IdentityUserManager.GetByIdAsync(id);
            var normlizedPermissions = new List<string>();

            await UserPermissionAppService.SetGrantedPermissionsAsync(user, normlizedPermissions);
        }


        public string CreateRandomPassword()
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

        protected virtual async Task UpdateUserByInput(IdentityUser user, UserEditDto input, ProfileChangedEvent changedEvent)
        {
            if (!string.Equals(user.UserName, input.UserName, StringComparison.OrdinalIgnoreCase))
            {
                (await IdentityUserManager.SetUserNameAsync(user, input.UserName)).CheckErrors();
                changedEvent.ChangedFields.Add(nameof(user.UserName));
            }
            if (!string.Equals(user.Email, input.Email, StringComparison.InvariantCultureIgnoreCase))
            {
                (await IdentityUserManager.SetEmailAsync(user, input.Email)).CheckErrors();
                changedEvent.ChangedFields.Add(nameof(user.Email));
            }
            if (!string.Equals(user.PhoneNumber, input.PhoneNumber, StringComparison.InvariantCultureIgnoreCase))
            {
                (await IdentityUserManager.SetPhoneNumberAsync(user, input.PhoneNumber)).CheckErrors();
                changedEvent.ChangedFields.Add(nameof(user.PhoneNumber));
            }
            if (!string.Equals(user.Name, input.Name))
            {
                user.Name = input.Name;
                changedEvent.ChangedFields.Add(nameof(user.Name));
            }
            if (!string.Equals(user.Surname, input.Surname))
            {
                user.Surname = input.Surname;
                changedEvent.ChangedFields.Add(nameof(user.Surname));
            }

            (await IdentityUserManager.SetLockoutEnabledAsync(user, input.LockoutEnabled)).CheckErrors();

            if (input.RoleNames != null)
            {
                (await IdentityUserManager.SetRolesAsync(user, input.RoleNames)).CheckErrors();
            }

            if (input.ShouldChangePasswordOnNextLogin)
            {
                user.SetProperty(nameof(UserEditDto.ShouldChangePasswordOnNextLogin), input.ShouldChangePasswordOnNextLogin);
            }
            else
            {
                user.RemoveProperty(nameof(UserEditDto.ShouldChangePasswordOnNextLogin));
            }
        }
    }
}
