using System;
using System.ComponentModel.DataAnnotations;

namespace PolpAbp.ZeroAdaptors.Organizations.Dto
{
    public class RolesToOrganizationUnitInput
    {
        public Guid[] RoleIds { get; set; }

        public Guid OrganizationUnitId { get; set; }
    }
}