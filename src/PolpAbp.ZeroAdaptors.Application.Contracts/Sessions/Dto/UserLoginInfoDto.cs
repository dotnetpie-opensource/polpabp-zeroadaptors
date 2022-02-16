using System;
using Volo.Abp.Application.Dtos;

namespace PolpAbp.ZeroAdaptors.Sessions.Dto
{
    public class UserLoginInfoDto : EntityDto<Guid>
    {
        public string Name { get; set; }

        public string Surname { get; set; }

        public string UserName { get; set; }

        public string EmailAddress { get; set; }

        public string ProfilePictureId { get; set; }
    }
}
