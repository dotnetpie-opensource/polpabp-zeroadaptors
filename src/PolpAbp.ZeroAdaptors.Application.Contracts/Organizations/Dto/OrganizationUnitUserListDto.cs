using System;
using Volo.Abp.Application.Dtos;

namespace PolpAbp.ZeroAdaptors.Organizations.Dto
{
    public class OrganizationUnitUserListDto : EntityDto<Guid>
    {
        public string Name { get; set; }

        public string Surname { get; set; }

        public string UserName { get; set; }

        public string EmailAddress { get; set; }

        public Guid? ProfilePictureId { get; set; }

        public DateTime AddedTime { get; set; }
    }
}