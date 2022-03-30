using System;
using System.Collections.Generic;
using System.Text;
using Volo.Abp.Application.Dtos;

namespace PolpAbp.ZeroAdaptors.Authorization.Roles.Dto
{
    public class FindRoleMembersInput : PagedAndSortedResultRequestDto
    {
        public Guid RoleId { get; set; }
        public string Filter { get; set; }
    }
}
