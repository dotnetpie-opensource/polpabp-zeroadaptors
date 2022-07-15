using System;
using System.Collections.Generic;
using Volo.Abp.Application.Dtos;
using Volo.Abp.Data;

namespace PolpAbp.ZeroAdaptors.Organizations.Dto
{
    public class OrganizationUnitDto : AuditedEntityDto<Guid>, IHasExtraProperties
    {
        public Guid? ParentId { get; set; }

        public string Code { get; set; }

        public string DisplayName { get; set; }

        public int MemberCount { get; set; }
        
        public int RoleCount { get; set; }

        public ExtraPropertyDictionary ExtraProperties { get; set; }

        public OrganizationUnitDto()
        {
            ExtraProperties = new ExtraPropertyDictionary();
        }
    }
}