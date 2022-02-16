using System.Collections.Generic;
using System.Threading.Tasks;
using System.Linq;
using Volo.Abp;
using Volo.Abp.Authorization.Permissions;
using Volo.Abp.Identity;
using Volo.Abp.PermissionManagement;

namespace PolpAbp.ZeroAdaptors.Authorization.Permissions
{
    [RemoteService(false)]
    public class UserPermissionAppService : ZeroAdaptorsAppService, IUserPermissionAppService
    {
        private readonly IPermissionStore _permissionStore;
        private readonly IdentityRoleStore _identityRoleStore;
        private readonly IPermissionManager _permissionManager;

        public UserPermissionAppService(IPermissionStore permissionStore, 
            IdentityRoleStore identityRoleStore,
            IPermissionManager permissionManager)
        {
            _permissionStore = permissionStore;
            _identityRoleStore = identityRoleStore;
            _permissionManager = permissionManager;
        }

        public async Task<List<string>> GetGrantedPermissionsAsync(IdentityUser user, List<string> allPermissions)
        {
            var ret = new List<string>();
            var userId = user.Id.ToString();
            var roleNames = new List<string>();
            
            foreach(var r in user.Roles)
            {
                var role = await _identityRoleStore.FindByIdAsync(r.RoleId.ToString());
                roleNames.Add(role.NormalizedName);
            }

            // First of all 
            foreach(var p in allPermissions)
            {
                if (await IsGrantedByAnyRoleAsync(p, roleNames))
                {
                    ret.Add(p);
                    continue;
                } 

                if (await IsGrantedForJustThisUserAsync(p, userId))
                {
                    ret.Add(p);
                    continue;
                }
            }

            return ret;
        }

        public async Task SetGrantedPermissionsAsync(IdentityUser user, List<string> expectedPermissions)
        {
            var permissionsByRoles = new List<string>();
            var roleNames = new List<string>();

            // If any permission is granted by any role, we cannot remove it. 
            foreach (var r in user.Roles)
            {
                var role = await _identityRoleStore.FindByIdAsync(r.RoleId.ToString());
                roleNames.Add(role.NormalizedName);
            }

            foreach (var p in expectedPermissions)
            {
                if (await IsGrantedByAnyRoleAsync(p, roleNames))
                {
                    permissionsByRoles.Add(p);
                    continue;
                }
            }

            // The list of permissions implicitly granted by any role cannot be changed yet.
            var permissionsByUserProvider = expectedPermissions.Where(a => !permissionsByRoles.Any(b => b == a)).ToList();

            // Compute the list of permissions to be removed 
            // and the list of permissions to be granted
            var userId = user.Id.ToString();
            var grantedByUsers = await _permissionManager.GetAllAsync(UserPermissionValueProvider.ProviderName, userId);
            grantedByUsers = grantedByUsers.Where(x => x.IsGranted).ToList();

            // Find out the permissions that are not in the given list 
            var tobeRemoved = grantedByUsers.Where(x => !permissionsByUserProvider.Any(y => y == x.Name));
            var tobeAdded = permissionsByUserProvider.Where(x => !grantedByUsers.Any(y => y.Name == x));

            foreach(var x in tobeRemoved)
            {
                await _permissionManager.SetAsync(x.Name, UserPermissionValueProvider.ProviderName, userId, false);
            }

            foreach (var x in tobeAdded)
            {
                await _permissionManager.SetAsync(x, UserPermissionValueProvider.ProviderName, userId, true);
            }
        }

        protected async Task<bool> IsGrantedForJustThisUserAsync(string permissioName, string userId)
        {
            return await _permissionStore.IsGrantedAsync(permissioName, "U", userId);
        }

        protected async Task<bool> IsGrantedByAnyRoleAsync(string permissionName, List<string> roles)
        {
            foreach (var role in roles)
            {
                if (await _permissionStore.IsGrantedAsync(permissionName, "R", role))
                {
                    return true;
                }
            }
            return false;
        }

    }
}
