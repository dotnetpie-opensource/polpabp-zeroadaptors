using Microsoft.AspNetCore.Identity;
using PolpAbp.Framework.Authorization.Users;
using PolpAbp.Framework.Authorization.Users.Events;
using PolpAbp.Framework.Security;
using PolpAbp.ZeroAdaptors.Authorization.Users.Dto;
using PolpAbp.ZeroAdaptors.Authorization.Users.Profile.Dto;
using System;
using System.Threading.Tasks;
using Volo.Abp;
using Volo.Abp.EventBus.Local;
using IdentityUserManager = Volo.Abp.Identity.IdentityUserManager;

namespace PolpAbp.ZeroAdaptors.Authorization.Users.Profile
{

    [RemoteService(false)]
    public class ProfileAppService : ZeroAdaptorsAppService, IProfileAppService
    {
        protected readonly IdentityUserManager IdentityUserManager;
        protected readonly IUserIdentityAssistantAppService UserIdentityAssistantAppService;
        protected readonly ILocalEventBus LocalEventBus;

        public ProfileAppService(IdentityUserManager identityUserManager, 
            ILocalEventBus localEventBus,
            IUserIdentityAssistantAppService userIdentityAssistantAppService)
        {
            IdentityUserManager = identityUserManager;
            LocalEventBus = localEventBus;
            UserIdentityAssistantAppService = userIdentityAssistantAppService;
        }

        public Task ChangeLanguage(ChangeUserLanguageDto input)
        {
            throw new NotImplementedException();
        }

        public async Task<IdentityResult> ChangePasswordAsync(ChangePasswordInput input)
        {
            var user = await IdentityUserManager.GetByIdAsync(CurrentUser.Id.Value);
            var ret = await IdentityUserManager.ChangePasswordAsync(user, input.CurrentPassword, input.NewPassword);
            if (ret.Succeeded)
            {
                await LocalEventBus.PublishAsync(
                    new PasswordChangedEvent
                    {
                        TenantId = user.TenantId,
                        UserId = user.Id
                    });
            }
            return ret;
        }

        public async Task<CurrentUserProfileEditDto> GetCurrentUserProfileForEditAsync()
        {
            var user = await IdentityUserManager.GetByIdAsync(CurrentUser.Id.Value);
            // todo: Move to automap?
            return new CurrentUserProfileEditDto
            {
                EmailAddress = user.Email,
                UserName = user.UserName,
                Surname = user.Surname,
                Name = user.Name,
                PhoneNumber = user.PhoneNumber,
                IsPhoneNumberConfirmed = user.PhoneNumberConfirmed
            };
        }

        public Task<GetProfilePictureOutput> GetFriendProfilePictureById(GetFriendProfilePictureByIdInput input)
        {
            throw new NotImplementedException();
        }

        public async Task<GetPasswordComplexitySettingOutput> GetPasswordComplexitySettingAsync()
        {
            // TODO: Read from setting manager
            var output = new GetPasswordComplexitySettingOutput();
            output.Setting = await UserIdentityAssistantAppService.ReadInPasswordComplexityAsync();
            return output;
        }

        public Task<GetProfilePictureOutput> GetProfilePicture()
        {
            throw new NotImplementedException();
        }

        public Task<GetProfilePictureOutput> GetProfilePictureById(Guid profilePictureId)
        {
            throw new NotImplementedException();
        }

        public Task PrepareCollectedData()
        {
            throw new NotImplementedException();
        }

        public Task SendVerificationSms(SendVerificationSmsInputDto input)
        {
            throw new NotImplementedException();
        }

        public async Task UpdateCurrentUserProfileAsync(CurrentUserProfileEditDto input)
        {
            var user = await IdentityUserManager.GetByIdAsync(CurrentUser.Id.Value);
            var changedEvent = new ProfileChangedEvent
            {
                TenantId = user.TenantId,
                UserId = user.Id
            };
            // Update user name
            if (!string.Equals(user.UserName, input.UserName, StringComparison.OrdinalIgnoreCase))
            {
                (await IdentityUserManager.SetUserNameAsync(user, input.UserName)).CheckErrors();
                changedEvent.ChangedFields.Add(nameof(user.UserName));
            }
            if (!string.Equals(user.Email, input.EmailAddress, StringComparison.OrdinalIgnoreCase))
            {
                (await IdentityUserManager.SetEmailAsync(user, input.EmailAddress)).CheckErrors();
                changedEvent.ChangedFields.Add(nameof(user.Email));
            }
            if (!string.IsNullOrEmpty(input.PhoneNumber))
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

            (await IdentityUserManager.UpdateAsync(user)).CheckErrors();
            await LocalEventBus.PublishAsync(changedEvent);
        }

        public Task<UpdateGoogleAuthenticatorKeyOutput> UpdateGoogleAuthenticatorKey()
        {
            throw new NotImplementedException();
        }

        public Task UpdateProfilePicture(UpdateProfilePictureInput input)
        {
            throw new NotImplementedException();
        }

        public Task VerifySmsCode(VerifySmsCodeInputDto input)
        {
            throw new NotImplementedException();
        }
    }
}
