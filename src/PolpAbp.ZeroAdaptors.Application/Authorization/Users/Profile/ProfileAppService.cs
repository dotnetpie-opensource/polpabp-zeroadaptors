using System;
using System.Threading.Tasks;
using PolpAbp.ZeroAdaptors.Authorization.Users.Dto;
using PolpAbp.ZeroAdaptors.Authorization.Users.Profile.Dto;
using Volo.Abp;

namespace PolpAbp.ZeroAdaptors.Authorization.Users.Profile
{

    [RemoteService(false)]
    public class ProfileAppService : ZeroAdaptorsAppService, IProfileAppService
    {
        public Task ChangeLanguage(ChangeUserLanguageDto input)
        {
            throw new NotImplementedException();
        }

        public Task ChangePassword(ChangePasswordInput input)
        {
            throw new NotImplementedException();
        }

        public Task<CurrentUserProfileEditDto> GetCurrentUserProfileForEdit()
        {
            throw new NotImplementedException();
        }

        public Task<GetProfilePictureOutput> GetFriendProfilePictureById(GetFriendProfilePictureByIdInput input)
        {
            throw new NotImplementedException();
        }

        public Task<GetPasswordComplexitySettingOutput> GetPasswordComplexitySetting()
        {
            return Task.Run(() =>
            {
                // TODO: Read from setting manager
                var output = new GetPasswordComplexitySettingOutput();
                output.Setting = new PolpAbp.ZeroAdaptors.Security.PasswordComplexitySetting();
                return output;
            });
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

        public Task UpdateCurrentUserProfile(CurrentUserProfileEditDto input)
        {
            throw new NotImplementedException();
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
