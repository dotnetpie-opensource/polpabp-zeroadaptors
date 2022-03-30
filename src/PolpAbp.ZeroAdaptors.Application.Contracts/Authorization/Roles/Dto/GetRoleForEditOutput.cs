using System.Collections.Generic;
using PolpAbp.ZeroAdaptors.Authorization.Permissions.Dto;

namespace PolpAbp.ZeroAdaptors.Authorization.Roles.Dto
{
    public class GetRoleForEditOutput
    {
        public RoleEditDto Role { get; set; }

        public List<FlatPermissionDto> Permissions { get; set; }

        public List<string> GrantedPermissionNames { get; set; }
    }
}