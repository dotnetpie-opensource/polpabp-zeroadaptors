using System;
using System.Collections.Generic;
using System.Text;

namespace PolpAbp.ZeroAdaptors.Authorization.Users.Dto
{
    public class UserRoleDto
    {
        public Guid RoleId { get; set; }

        public string RoleName { get; set; }

        public string RoleDisplayName { get; set; }

        public bool IsAssigned { get; set; }

        public bool InheritedFromOrganizationUnit { get; set; }
    }
}
