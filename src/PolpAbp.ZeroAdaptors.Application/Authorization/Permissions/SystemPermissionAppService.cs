using Microsoft.Extensions.Options;
using PolpAbp.ZeroAdaptors.Authorization.Permissions.Dto;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Volo.Abp.Authorization.Permissions;
using Volo.Abp.DependencyInjection;
using Volo.Abp.Guids;
using Volo.Abp.Identity;
using Volo.Abp.MultiTenancy;
using PermissionManager = Volo.Abp.PermissionManagement.PermissionManager;

namespace PolpAbp.ZeroAdaptors.Authorization.Permissions
{
    /// <summary>
    /// Provides a singleton service to collect all of the permissions defined in the system.
    /// 
    /// Note that all of the permissions are perdefined by the system. (We do not 
    /// support any dynamic permissions yet.)
    /// 
    /// Also we on purpose expose the service as IPermissionAppService to 
    /// rule out any wrong DI.
    /// </summary>
    [ExposeServices(typeof(ISystemPermissionAppService))]
    public class SystemPermissionAppService : PermissionManager, ISystemPermissionAppService, ISingletonDependency
    {
        public SystemPermissionAppService(IPermissionDefinitionManager permissionDefinitionManager,
            Volo.Abp.PermissionManagement.IPermissionGrantRepository permissionGrantRepository,
            IServiceProvider serviceProvider,
            IGuidGenerator guidGenerator,
            IOptions<Volo.Abp.PermissionManagement.PermissionManagementOptions> options,
            ICurrentTenant currentTenant
            ) : base(permissionDefinitionManager, permissionGrantRepository, serviceProvider, guidGenerator, options, currentTenant)
        {
        }

        public Task<List<PermissionDefinition>> GetAllPermissionsAsync(bool excludingHost)
        {
            return Task.Run(() =>
            {
                // We only suppose pass nothing, so we do not compute the granted provider for permissions.
                var results = new List<FlatPermissionWithLevelDto>();

                // We exclude those permission for machine clients as well.
                var permDefs = PermissionDefinitionManager.GetPermissions()
                          .Where(x => x.IsEnabled && x.Name != IdentityPermissions.UserLookup.Default); 

                if (excludingHost)
                {
                    permDefs = permDefs.Where(x => x.MultiTenancySide.HasFlag(MultiTenancySides.Tenant));
                }

                // Sort permissions

                var sorted = permDefs.ToList();

                return sorted;
            });
        }

    }
}
