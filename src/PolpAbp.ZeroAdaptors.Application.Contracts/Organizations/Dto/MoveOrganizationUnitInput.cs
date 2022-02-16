using System;
using System.ComponentModel.DataAnnotations;

namespace PolpAbp.ZeroAdaptors.Organizations.Dto
{
    public class MoveOrganizationUnitInput
    {
        public Guid Id { get; set; }

        public Guid? NewParentId { get; set; }
    }
}