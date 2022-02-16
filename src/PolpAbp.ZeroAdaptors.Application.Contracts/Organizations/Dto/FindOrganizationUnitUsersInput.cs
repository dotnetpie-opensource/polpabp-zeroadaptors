
using System;
using Volo.Abp.Application.Dtos;

namespace PolpAbp.ZeroAdaptors.Organizations.Dto
{
    // todo
    public class FindOrganizationUnitUsersInput : PagedAndSortedResultRequestDto
    {
        public Guid OrganizationUnitId { get; set; }
        public string Filter { get; set; }

    }
}
