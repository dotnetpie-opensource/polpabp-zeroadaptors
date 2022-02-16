using System;
using Volo.Abp.Application.Dtos;

namespace PolpAbp.ZeroAdaptors.Organizations.Dto
{
    public class OrganizationUnitDto : AuditedEntityDto<Guid>
    {
        public Guid? ParentId { get; set; }

        public string Code { get; set; }

        public string DisplayName { get; set; }

        public int MemberCount { get; set; }
        
        public int RoleCount { get; set; }
    }
}