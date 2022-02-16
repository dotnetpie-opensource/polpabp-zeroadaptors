using System;
using System.ComponentModel.DataAnnotations;

namespace PolpAbp.ZeroAdaptors.Organizations.Dto
{
    public class RoleToOrganizationUnitInput
    {
        public Guid RoleId { get; set; }

        public Guid OrganizationUnitId { get; set; }
    }
}