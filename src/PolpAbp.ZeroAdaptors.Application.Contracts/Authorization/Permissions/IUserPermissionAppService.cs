using System.Collections.Generic;
using System.Threading.Tasks;
using Volo.Abp.Application.Services;
using Volo.Abp.Identity;

namespace PolpAbp.ZeroAdaptors.Authorization.Permissions
{
    /// <summary>
    /// Provides the service for computing the granted permission for 
    /// a user. 
    /// 
    /// Note that this service is transient.
    /// </summary>
    public interface IUserPermissionAppService : IApplicationService
    {
        /// <summary>
        /// Filters out the list of permissions that apply to the given user.
        ///
        /// Note that we cannot have PermissionDefinition as the type of allPermission,
        /// if we use it, an exception "Method may only be called on a Type for which Type.IsGenericParameter" is thrown.
        /// </summary>
        /// <param name="user">User</param>
        /// <param name="allPermissions">List of all possible permissions</param>
        /// <returns>List of granted permissions</returns>
        Task<List<string>> GetGrantedPermissionsAsync(IdentityUser user, List<string> allPermissions);
        Task SetGrantedPermissionsAsync(IdentityUser user, List<string> expectedPermissions);
    }
}
