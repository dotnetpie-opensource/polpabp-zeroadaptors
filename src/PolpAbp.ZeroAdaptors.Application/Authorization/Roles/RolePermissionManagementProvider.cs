using System;
using System.Collections.Generic;
using System.Text;
using Volo.Abp.Authorization.Permissions;
using Volo.Abp.Guids;
using Volo.Abp.MultiTenancy;
using Volo.Abp.PermissionManagement;

namespace PolpAbp.ZeroAdaptors.Authorization.Roles
{
    public class RolePermissionManagementProvider : PermissionManagementProvider
    {
        public RolePermissionManagementProvider(IPermissionGrantRepository permissionGrantRepository, 
            IGuidGenerator guidGenerator, 
            ICurrentTenant currentTenant) : base(permissionGrantRepository, guidGenerator, currentTenant)
        {
        }

        public override string Name => RolePermissionValueProvider.ProviderName;
    }
}
