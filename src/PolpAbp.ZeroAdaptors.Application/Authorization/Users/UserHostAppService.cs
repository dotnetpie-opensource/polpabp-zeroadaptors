using Microsoft.AspNetCore.Identity;
using PolpAbp.ZeroAdaptors.Authorization.Users.Dto;
using PolpAbp.ZeroAdaptors.Authorization.Users.Events;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Volo.Abp;
using Volo.Abp.EventBus.Local;
using Volo.Abp.Identity;

namespace PolpAbp.ZeroAdaptors.Authorization.Users
{
    [RemoteService(false)]
    public class UserHostAppService : ZeroAdaptorsAppService, IUserHostAppService
    {
        protected readonly IdentityUserManager IdentityUserManager;
        protected readonly ILocalEventBus LocalEventBus;
        protected readonly IUserIdentityAssistantAppService UserIdentityAssistantAppService;

        public UserHostAppService(IdentityUserManager identityUserManager,
            ILocalEventBus localEventBus,
            IUserIdentityAssistantAppService userIdentityAssistantAppService)
        {
            IdentityUserManager = identityUserManager;
            LocalEventBus = localEventBus;
            UserIdentityAssistantAppService = userIdentityAssistantAppService;
        }

        public async Task ResetUserPasswordAsync(Guid? tenantId, Guid userId, ResetUserPasswordDto input)
        {
            using (CurrentTenant.Change(tenantId))
            {
                var user = await IdentityUserManager.GetByIdAsync(userId);

                //Set password
                if (input.SetRandomPassword)
                {
                    var randomPassword = await UserIdentityAssistantAppService.CreateRandomPasswordAsync();
                    input.Password = randomPassword;
                }

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
        }
    }
}
