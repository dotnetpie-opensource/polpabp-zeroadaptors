using System;
using System.ComponentModel.DataAnnotations;

namespace PolpAbp.ZeroAdaptors.Organizations.Dto
{
    public class UpdateOrganizationUnitInput
    {
        public Guid Id { get; set; }

        // Please refer to Volo.Abp.Identity.Settings.IdentitySettingNames;
        [Required]
        [StringLength(128 /* OrganizationUnit.MaxDisplayNameLength */)]
        public string DisplayName { get; set; }
    }
}