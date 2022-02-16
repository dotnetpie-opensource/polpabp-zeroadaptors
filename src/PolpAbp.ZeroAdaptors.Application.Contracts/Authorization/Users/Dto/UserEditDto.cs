using System;
using System.ComponentModel.DataAnnotations;
using Volo.Abp.Auditing;
using Volo.Abp.Identity;
using Volo.Abp.Validation;

namespace PolpAbp.ZeroAdaptors.Authorization.Users.Dto
{
    public class UserEditDto 
    {
        /// <summary>
        /// Set null to create a new user. Set user's Id to update a user
        /// </summary>
        public Guid? Id { get; set; }

        // We have to copy the fields form IdentityUserUpdateDto, 
        // because Email is required in it and but the client cannot meet up with this requirement.

        [DynamicStringLength(typeof(IdentityUserConsts), "MaxUserNameLength", null)]
        [Required]
        public string UserName { get; set; }
        [DynamicStringLength(typeof(IdentityUserConsts), "MaxNameLength", null)]
        public string Name { get; set; }
        [DynamicStringLength(typeof(IdentityUserConsts), "MaxSurnameLength", null)]
        public string Surname { get; set; }
        [DynamicStringLength(typeof(IdentityUserConsts), "MaxEmailLength", null)]
        public string Email { get; set; }
        [DynamicStringLength(typeof(IdentityUserConsts), "MaxPhoneNumberLength", null)]
        public string PhoneNumber { get; set; }
        public bool LockoutEnabled { get; set; }
        public string[] RoleNames { get; set; }

        [DisableAuditing]
        [DynamicStringLength(typeof(IdentityUserConsts), "MaxPasswordLength", null)]
        public string Password { get; set; }
        public string ConcurrencyStamp { get; set; }

        [Required]
        [EmailAddress]
        public string EmailAddress { get; set; }

        public bool IsActive { get; set; }

        public bool ShouldChangePasswordOnNextLogin { get; set; }

        public virtual bool IsTwoFactorEnabled { get; set; }

        public virtual bool IsLockoutEnabled { get; set; }
    }
}
