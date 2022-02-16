using System.ComponentModel.DataAnnotations;

namespace PolpAbp.ZeroAdaptors.Authorization.Accounts.Dto
{
    public class SendEmailActivationLinkInput
    {
        [Required]
        public string EmailAddress { get; set; }
    }
}