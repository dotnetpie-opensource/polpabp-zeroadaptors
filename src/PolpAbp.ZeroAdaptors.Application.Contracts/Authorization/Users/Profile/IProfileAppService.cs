using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using PolpAbp.ZeroAdaptors.Authorization.Users.Dto;
using PolpAbp.ZeroAdaptors.Authorization.Users.Profile.Dto;
using PolpAbp.ZeroAdaptors.Authorization.Users.Events;
using Volo.Abp.Application.Services;

namespace PolpAbp.ZeroAdaptors.Authorization.Users.Profile
{
    // This service corresponds to the one in
    // Application.Shared/Authorization.Users.Profile
    /// <summary>
    /// Provides the service for managing the profile of a user.
    /// </summary>
    public interface IProfileAppService : IApplicationService
    {
        /// <summary>
        /// Builds the user profile model for the authenticated user.
        /// </summary>
        /// <returns>User profile</returns>
        Task<CurrentUserProfileEditDto> GetCurrentUserProfileForEditAsync();

        /// <summary>
        /// Updates the user profile for the authenticated.
        /// Once the operation is successful, it emits  
        /// <see cref="ProfileChangedEvent"> an event </see>.
        /// Therefore, a client may define a handler to process this event for 
        /// its business needs.
        /// </summary>
        /// <param name="input">Updated user profile</param>
        /// <returns>No</returns>
        Task UpdateCurrentUserProfileAsync(CurrentUserProfileEditDto input);

        /// <summary>
        /// Updates the password for the authenticated user.
        /// Once the operation is successful, it emits  
        /// <see cref="PasswordChangedEvent"> an event </see>.
        /// Therefore, a client may define a handler to process this event for 
        /// its business needs.
        /// </summary>
        /// <param name="input">Password info.</param>
        /// <returns>Identity result</returns>
        Task<IdentityResult> ChangePasswordAsync(ChangePasswordInput input);

        Task UpdateProfilePicture(UpdateProfilePictureInput input);

        Task<GetPasswordComplexitySettingOutput> GetPasswordComplexitySettingAsync();

        Task<GetProfilePictureOutput> GetProfilePicture();

        Task<GetProfilePictureOutput> GetProfilePictureById(Guid profilePictureId);

        Task<GetProfilePictureOutput> GetFriendProfilePictureById(GetFriendProfilePictureByIdInput input);

        Task ChangeLanguage(ChangeUserLanguageDto input);

        Task<UpdateGoogleAuthenticatorKeyOutput> UpdateGoogleAuthenticatorKey();

        Task SendVerificationSms(SendVerificationSmsInputDto input);

        Task VerifySmsCode(VerifySmsCodeInputDto input);

        Task PrepareCollectedData();
    }
}
