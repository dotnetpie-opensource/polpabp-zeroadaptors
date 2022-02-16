using System;
using System.ComponentModel.DataAnnotations;

namespace PolpAbp.ZeroAdaptors.Organizations.Dto
{
    public class UserToOrganizationUnitInput
    {
        public Guid UserId { get; set; }

        public Guid OrganizationUnitId { get; set; }
    }
}