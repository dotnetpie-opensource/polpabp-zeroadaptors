using System;
using Volo.Abp.Application.Dtos;

namespace PolpAbp.ZeroAdaptors.Organizations.Dto
{
    public class OrganizationUnitRoleListDto : EntityDto<Guid>
    {
        public string DisplayName { get; set; }

        public string Name { get; set; }
        
        public DateTime AddedTime { get; set; }
    }
}