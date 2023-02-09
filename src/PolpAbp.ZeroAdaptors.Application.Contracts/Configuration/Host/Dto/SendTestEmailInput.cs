using System.ComponentModel.DataAnnotations;

namespace PolpAbp.ZeroAdaptors.Configuration.Host.Dto
{
    public class SendTestEmailInput
    {
        [Required]
        // [MaxLength(AbpUserBase.MaxEmailAddressLength)]
        public string EmailAddress { get; set; }
    }
}