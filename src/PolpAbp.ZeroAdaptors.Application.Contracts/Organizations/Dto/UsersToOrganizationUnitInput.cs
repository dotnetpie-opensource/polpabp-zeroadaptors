using System;
using System.ComponentModel.DataAnnotations;

namespace PolpAbp.ZeroAdaptors.Organizations.Dto
{
    public class UsersToOrganizationUnitInput
    {
        public Guid[] UserIds { get; set; }

        public Guid OrganizationUnitId { get; set; }
    }
}