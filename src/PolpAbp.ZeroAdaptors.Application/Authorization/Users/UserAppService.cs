using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using PolpAbp.Framework.Identity;
using PolpAbp.ZeroAdaptors.Authorization.Permissions;
using PolpAbp.ZeroAdaptors.Authorization.Permissions.Dto;
using PolpAbp.ZeroAdaptors.Authorization.Users.Dto;
using PolpAbp.ZeroAdaptors.Authorization.Users.Events;
using PolpAbp.ZeroAdaptors.Organizations.Dto;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Volo.Abp;
using Volo.Abp.Data;
using Volo.Abp.EventBus.Local;
using Volo.Abp.Identity;
using IdentityUser = Volo.Abp.Identity.IdentityUser;

namespace PolpAbp.ZeroAdaptors.Authorization.Users
{
    [RemoteService(false)]
    public class UserAppService : ZeroAdaptorsAppService, IUserAppService
    {

        protected readonly IIdentityRoleRepository IdentityRoleRepository;
        protected readonly IOrganizationUnitRepository OrganizationUnitRepository;
        protected readonly IdentityUserManager IdentityUserManager;
        protected readonly IIdentityUserRepository IdentityUserRepository;
        protected readonly IRegisteredUserDataSeeder RegisteredUserDataSeeder;
        protected readonly ISystemPermissionAppService SystemPermissionAppService;
        protected readonly IUserPermissionAppService UserPermissionAppService;
        protected readonly ILocalEventBus LocalEventBus;
        protected readonly IUserIdentityAssistantAppService UserIdentityAssistantAppService;

        public UserAppService(IIdentityRoleRepository identityRoleRepository,
            IOrganizationUnitRepository organizationUnitRepository,
            IdentityUserManager identityUserManager,
            IIdentityUserRepository identityUserRepository,
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
            RegisteredUserDataSeeder = registeredUserDataSeeder;
            SystemPermissionAppService = systemPermissionAppService;
            UserPermissionAppService = userPermissionAppService;
            LocalEventBus = localEventBus;
            UserIdentityAssistantAppService = userIdentityAssistantAppService;
        }

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
                EmailAddress = user.Email, // Client expect email address
                Name = user.Name,
                Surname = user.Surname,
                UserName = user.UserName,
                PhoneNumber = user.PhoneNumber,
                IsTwoFactorEnabled = user.TwoFactorEnabled,
                IsLockoutEnabled = user.LockoutEnabled,
                ShouldChangePasswordOnNextLogin = user.ShouldChangePasswordOnNextLogin(),
                IsActive = user.IsActive
                // todo:
                // ....
            };
            // Extra properties 
            foreach (var p in user.ExtraProperties)
            {
                output.User.ExtraProperties.Add(p.Key, p.Value);
            }

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

        public async Task<Guid> CreateUserAsync(CreateOrUpdateUserInput input, 
            Action<IdentityUser> extraCallback = null)
        {
            // todo: Check max user restriction 


            // Normalize values so that we can leverage the helper 
            // method to set up roles and other. 
            input.User.RoleNames = input.AssignedRoleNames;
            //Set password
            if (input.SetRandomPassword)
            {
                var randomPassword = await UserIdentityAssistantAppService.CreateRandomPasswordAsync();
                // Generated password should conform to our restrication
                input.User.Password = randomPassword;
            }
            else
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

            (await IdentityUserManager.CreateAsync(user, input.User.Password)).CheckErrors();

            var changedEvent = new ProfileChangedEvent()
            {
                TenantId = CurrentTenant.Id,
                OperatorId = CurrentUser?.Id,
                UserId  = user.Id,
                SendActivationEmail = input.SendActivationEmail
            }; 
            await UpdateUserByInput(user, input.User, changedEvent);

            foreach (var p in input.User.ExtraProperties)
            {
                if (p.Value == null || string.IsNullOrEmpty(p.Value.ToString()))
                {
                    user.RemoveProperty(p.Key);
                }
                else
                {
                    user.SetProperty(p.Key, p.Value);
                }
            }

            if (extraCallback != null)
            {
                extraCallback.Invoke(user);
            }

            (await IdentityUserManager.UpdateAsync(user)).CheckErrors();

            await CurrentUnitOfWork.SaveChangesAsync(); // Next make sense to send notifications 

            // Add into org.
            await IdentityUserManager.SetOrganizationUnitsAsync(user, input.OrganizationUnits.ToArray());

            // todo: Any seed data ????
            // Next seed data for this user, e.g., user role and so on.
            await RegisteredUserDataSeeder.SeedAsync(input.User.EmailAddress, CurrentTenant.Id);

            await LocalEventBus.PublishAsync(changedEvent);

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

        public async Task UpdateUserAsync(CreateOrUpdateUserInput input,
             Action<IdentityUser> extraCallback = null)
        {

            // Normalize values so that we can leverage the helper 
            // method to set up roles and other. 
            input.User.RoleNames = input.AssignedRoleNames;

            var user = await IdentityUserManager.GetByIdAsync(input.User.Id.Value);

            var changedEvent = new ProfileChangedEvent()
            {
                TenantId = CurrentTenant.Id,
                OperatorId = CurrentUser?.Id,
                SendActivationEmail = input.SendActivationEmail,
                UserId = user.Id
            };

            // todo: ??
            // user.ConcurrencyStamp = input.ConcurrencyStamp;
            await UpdateUserByInput(user, input.User, changedEvent);
            // todo: 
            // input.MapExtraPropertiesTo(user);
            foreach (var p in input.User.ExtraProperties)
            {
                if (p.Value == null || string.IsNullOrEmpty(p.Value.ToString()))
                {
                    user.RemoveProperty(p.Key);
                }
                else
                {
                    user.SetProperty(p.Key, p.Value);
                }
            }

            extraCallback?.Invoke(user);

            (await IdentityUserManager.UpdateAsync(user)).CheckErrors();

            //Set password
            if (input.SetRandomPassword)
            {
                var randomPassword = await UserIdentityAssistantAppService.CreateRandomPasswordAsync();
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

            await LocalEventBus.PublishAsync(changedEvent);

            // Events 
            if (passwordChangedEvent != null)
            {
                await LocalEventBus.PublishAsync(passwordChangedEvent);
            }
        }

        public async Task ResetUserPasswordAsync(Guid id, ResetUserPasswordDto input, bool runValidator)
        {
            var user = await IdentityUserManager.GetByIdAsync(id);

            //Set password
            if (input.SetRandomPassword)
            {
                var randomPassword = await UserIdentityAssistantAppService.CreateRandomPasswordAsync();
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

            if (input.ShouldChangePasswordOnNextLogin)
            {
                user.SetShouldChangePasswordOnNextLogin();
                await IdentityUserManager.UpdateAsync(user);
            }

            var passwordChangedEvent = new PasswordChangedEvent()
            {
                UserId = user.Id,
                OperatorId = CurrentUser?.Id,
                TenantId = CurrentUser.TenantId,
                NewPassword = input.Password
            };

            await LocalEventBus.PublishAsync(passwordChangedEvent);
        }

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

        public async Task UpdateUserPermissionsAsync(UpdateUserPermissionsInput input)
        {
            var user = await IdentityUserManager.GetByIdAsync(input.Id);
            var normlizedPermissions = input.GrantedPermissionNames
                .Select(x => x).ToList();

            await UserPermissionAppService.SetGrantedPermissionsAsync(user, normlizedPermissions);
        }

        public async Task ResetUserSpecificPermissionsAsync(Guid id)
        {
            var user = await IdentityUserManager.GetByIdAsync(id);
            var normlizedPermissions = new List<string>();

            await UserPermissionAppService.SetGrantedPermissionsAsync(user, normlizedPermissions);
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

            // Active
            user.SetIsActive(input.IsActive);

            if (input.ShouldChangePasswordOnNextLogin)
            {
                user.SetShouldChangePasswordOnNextLogin();
            }
            else
            {
                user.RemoveShouldChangePasswordOnNextLogin();
            }
        }
    }
}
