using System;
using System.ComponentModel.DataAnnotations;

namespace PolpAbp.ZeroAdaptors.Authorization.Roles.Dto
{
    public class RoleEditDto
    {
        public Guid? Id { get; set; }

        [Required]
        public string DisplayName { get; set; }
        
        public bool IsDefault { get; set; }
    }
}