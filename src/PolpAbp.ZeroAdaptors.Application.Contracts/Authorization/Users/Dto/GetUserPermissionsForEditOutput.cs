using System.Collections.Generic;
using PolpAbp.ZeroAdaptors.Authorization.Permissions.Dto;

namespace PolpAbp.ZeroAdaptors.Authorization.Users.Dto
{
    public class GetUserPermissionsForEditOutput
    {
        public List<FlatPermissionDto> Permissions { get; set; }

        public List<string> GrantedPermissionNames { get; set; }
    }
}
