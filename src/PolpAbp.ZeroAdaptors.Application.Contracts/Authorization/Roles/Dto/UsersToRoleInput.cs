using System;

namespace PolpAbp.ZeroAdaptors.Authorization.Roles.Dto
{
    public class UsersToRoleInput
    {
        public Guid[] UserIds { get; set; }

        public Guid RoleId { get; set; }
    }
}
