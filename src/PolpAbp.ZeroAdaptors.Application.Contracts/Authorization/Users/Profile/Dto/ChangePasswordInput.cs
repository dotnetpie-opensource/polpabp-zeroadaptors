using System.ComponentModel.DataAnnotations;
using Volo.Abp.Auditing;

namespace PolpAbp.ZeroAdaptors.Authorization.Users.Profile.Dto
{
    public class ChangePasswordInput
    {
        [Required]
        [DisableAuditing]
        public string CurrentPassword { get; set; }

        [Required]
        [DisableAuditing]
        public string NewPassword { get; set; }
    }
}