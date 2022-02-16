using System.ComponentModel.DataAnnotations;

namespace PolpAbp.ZeroAdaptors.Authorization.Users.Profile.Dto
{
    public class CurrentUserProfileEditDto
    {
        // TODO: Fix
        [Required]
        // [StringLength(AbpUserBase.MaxNameLength)]
        public string Name { get; set; }

        // TODO: Fix
        [Required]
        // [StringLength(AbpUserBase.MaxSurnameLength)]
        public string Surname { get; set; }

        // TODO: Fix
        [Required]
        // [StringLength(AbpUserBase.MaxUserNameLength)]
        public string UserName { get; set; }

        // TODO: Fix
        [Required]
        // [StringLength(AbpUserBase.MaxEmailAddressLength)]
        public string EmailAddress { get; set; }

        // TODO: Fix
        // [StringLength(UserConsts.MaxPhoneNumberLength)]
        public string PhoneNumber { get; set; }

        public virtual bool IsPhoneNumberConfirmed { get; set; }

        public string Timezone { get; set; }

        public string QrCodeSetupImageUrl { get; set; }

        public bool IsGoogleAuthenticatorEnabled { get; set; }
    }
}