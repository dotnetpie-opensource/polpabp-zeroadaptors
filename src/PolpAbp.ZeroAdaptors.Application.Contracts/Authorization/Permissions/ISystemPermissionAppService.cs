using System.Collections.Generic;
using System.Threading.Tasks;
using Volo.Abp.Authorization.Permissions;

namespace PolpAbp.ZeroAdaptors.Authorization.Permissions
{

    /// <summary>
    /// Provides a singleton service to collect all of the permissions defined in the system.
    /// 
    /// Note that all of the permissions are perdefined by the system. (We do not 
    /// support any dynamic permissions yet.)
    /// </summary>
    public interface ISystemPermissionAppService
    {
        /// <summary>
        /// Computes all of the system-defined permissions. 
        /// </summary>
        /// <param name="excludingHost">Do not include any permission for HOSt</param>
        /// <returns>A list of leveled permissions</returns>
        Task<List<PermissionDefinition>> GetAllPermissionsAsync(bool excludingHost);
    }
}
