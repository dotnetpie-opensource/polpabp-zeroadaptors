using System;
using System.ComponentModel.DataAnnotations;
using Volo.Abp.Identity;

namespace PolpAbp.ZeroAdaptors.Organizations.Dto
{
    public class CreateOrganizationUnitInput
    {
        // todo: Guid ?
        public Guid? ParentId { get; set; }

        // Please refer to Volo.Abp.Identity package for the definition of 
        // OrganizationUnitConsts.MaxDisplayNameLength
        [Required]
        [StringLength(128 /*OrganizationUnitConsts.MaxDisplayNameLength */)]
        public string DisplayName { get; set; } 
    }
}