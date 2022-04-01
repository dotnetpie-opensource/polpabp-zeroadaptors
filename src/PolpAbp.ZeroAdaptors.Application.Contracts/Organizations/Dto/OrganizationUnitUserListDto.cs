using System;
using System.Collections.Generic;
using Volo.Abp.Application.Dtos;
using Volo.Abp.Data;

namespace PolpAbp.ZeroAdaptors.Organizations.Dto
{
    public class OrganizationUnitUserListDto : EntityDto<Guid>, IHasExtraProperties
    {
        public string Name { get; set; }

        public string Surname { get; set; }

        public string UserName { get; set; }

        public string EmailAddress { get; set; }

        public Guid? ProfilePictureId { get; set; }

        public DateTime AddedTime { get; set; }

        public Dictionary<string, object> ExtraProperties { get; set; }

        public OrganizationUnitUserListDto()
        {
            ExtraProperties = new Dictionary<string, object>();
        }
    }
}