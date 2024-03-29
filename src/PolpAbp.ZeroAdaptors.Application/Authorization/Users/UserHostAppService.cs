﻿using Microsoft.AspNetCore.Identity;
using PolpAbp.ZeroAdaptors.Authorization.Users.Dto;
using PolpAbp.ZeroAdaptors.Authorization.Users.Events;
using System;
using System.Threading.Tasks;
using Volo.Abp;
using Volo.Abp.Data;
using Volo.Abp.EventBus.Local;
using Volo.Abp.Identity;
using Volo.Abp.MultiTenancy;
using IdentityUser = Volo.Abp.Identity.IdentityUser;

namespace PolpAbp.ZeroAdaptors.Authorization.Users
{
    [RemoteService(false)]
    public class UserHostAppService : ZeroAdaptorsAppService, IUserHostAppService
    {
        protected readonly IdentityUserManager IdentityUserManager;
        protected readonly ILocalEventBus LocalEventBus;
        protected readonly IUserIdentityAssistantAppService UserIdentityAssistantAppService;
        protected readonly IDataFilter DataFilter;

        public UserHostAppService(IdentityUserManager identityUserManager,
            ILocalEventBus localEventBus,
            IUserIdentityAssistantAppService userIdentityAssistantAppService,
            IDataFilter dataFilter)
        {
            IdentityUserManager = identityUserManager;
            LocalEventBus = localEventBus;
            UserIdentityAssistantAppService = userIdentityAssistantAppService;
            DataFilter = dataFilter;
        }

        public async Task<IdentityUser> GetUserAcrossSystem(string email)
        {
            using (DataFilter.Disable<IMultiTenant>())
            {
                var user = await IdentityUserManager.FindByEmailAsync(email);
                return user;
            }
        }

        public async Task ResetUserPasswordAsync(Guid? tenantId, Guid userId, ResetUserPasswordDto input, Guid? operatorId)
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

                if (input.ShouldChangePasswordOnNextLogin)
                {
                    user.SetShouldChangePasswordOnNextLogin();
                    await IdentityUserManager.UpdateAsync(user);
                }

                var passwordChangedEvent = new PasswordChangedEvent()
                {
                    UserId = user.Id,
                    OperatorId = operatorId,
                    TenantId = tenantId,
                    NewPassword = input.Password
                };

                await LocalEventBus.PublishAsync(passwordChangedEvent);
            }
        }
    }
}
