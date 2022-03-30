using System.Collections.Generic;

namespace PolpAbp.ZeroAdaptors.Authorization.Roles.Dto
{
    public class GetRolesInput
    {
        public List<string> Permissions { get; set; }
    }
}
