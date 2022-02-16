
using System;
using Volo.Abp.Application.Dtos;

namespace PolpAbp.ZeroAdaptors.Organizations.Dto
{
    public class FindOrganizationUnitRolesInput : PagedAndSortedResultRequestDto
    {
        public Guid OrganizationUnitId { get; set; }
        public string Filter { get; set; }

    }
}