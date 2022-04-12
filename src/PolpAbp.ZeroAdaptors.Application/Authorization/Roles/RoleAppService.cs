using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using PolpAbp.Framework.Common.Dto;
using PolpAbp.Framework.Identity;
using PolpAbp.ZeroAdaptors.Authorization.Permissions.Dto;
using PolpAbp.ZeroAdaptors.Authorization.Roles.Dto;
using Volo.Abp;
using Volo.Abp.Application.Dtos;
using Volo.Abp.Authorization.Permissions;
using Volo.Abp.Guids;
using Volo.Abp.Identity;
using Volo.Abp.MultiTenancy;
using Volo.Abp.PermissionManagement;
using IdentityRole = Volo.Abp.Identity.IdentityRole;

namespace PolpAbp.ZeroAdaptors.Authorization.Roles
{
    /// <summary>
    /// Application service that is used by 'role management' page.
    /// </summary>
    [RemoteService(false)]
    public class RoleAppService : ZeroAdaptorsAppService, IRoleAppService
    {
        private readonly IIdentityRoleAppService _identityRoleAppService;
        private readonly IPermissionManager _permissionManager;
        private readonly IdentityRoleManager _identityRoleManager;
        private readonly IPermissionDefinitionManager _permissionDefinitionManager;
        private readonly IPermissionStore _permissionStore;
        private readonly IGuidGenerator _guidGenerator;
        private readonly IIdentityUserRepositoryExt _identityUserRepositoryExt;
        private readonly IdentityUserManager _userManager;

        public RoleAppService(
            IIdentityRoleAppService identityRoleAppService,
            IdentityRoleManager identityRoleManager,
            IPermissionManager permissionManager,
            IPermissionDefinitionManager permissionDefinitionManager,
            IPermissionStore permissionStore,
            IGuidGenerator guidGenerator,
            IIdentityUserRepositoryExt identityUserRepositoryExt,
            IdentityUserManager userManager
            )
        {
            _identityRoleAppService = identityRoleAppService;
            _identityRoleManager = identityRoleManager;
            _permissionManager = permissionManager;
            _permissionDefinitionManager = permissionDefinitionManager;
            _permissionStore = permissionStore;
            _guidGenerator = guidGenerator;
            _identityUserRepositoryExt = identityUserRepositoryExt;
            _userManager = userManager;
        }

        [Authorize(IdentityPermissions.Roles.Default)]
        public async Task<ListResultDto<RoleListDto>> GetRoles(GetRolesInput input)
        {
            var output = await _identityRoleAppService.GetAllListAsync();

            var items = output.Items.Select(a =>
            {
                var b = ObjectMapper.Map<IdentityRoleDto, RoleListDto>(a);
                b.DisplayName = b.Name;
                b.CreationTime = b.CreationTime;
                return b;
            }).ToList();

            return new ListResultDto<RoleListDto>(items);
        }

        [Authorize(IdentityPermissions.Roles.Default)]
        public async Task<GetRoleForEditOutput> GetRoleForEdit(NullableIdDto<Guid> input)
        {
            // We exclude those permission for machine clients as well.
            var permDefs = _permissionDefinitionManager.GetPermissions()
                      .Where(x => x.IsEnabled &&
                      x.Name != IdentityPermissions.UserLookup.Default &&
                      x.MultiTenancySide.HasFlag(MultiTenancySides.Tenant));


            var grantedPermissions = new List<string>();
            var roleEditDto = new RoleEditDto();

            if (input.Id.HasValue) //Editing existing role?
            {
                var role = await _identityRoleManager.GetByIdAsync(input.Id.Value);
                ObjectMapper.Map<IdentityRole, RoleEditDto>(role, roleEditDto);

                await ComputeGrantedPermissionsForRoleAsync(role, grantedPermissions);
            }

            var permissions = permDefs.Select(x => new FlatPermissionDto
            {
                Name = x.Name,
                ParentName = x.Parent?.Name,
                DisplayName = x.DisplayName.Localize(StringLocalizerFactory)
            }).ToList();

            return new GetRoleForEditOutput
            {
                Role = roleEditDto,
                Permissions =  permissions,
                GrantedPermissionNames = grantedPermissions
            };
        }

        // Currently we do not support just a create or update.
        [Authorize(IdentityPermissions.Roles.Create)]
        [Authorize(IdentityPermissions.Roles.Update)]
        [Authorize(IdentityPermissions.Roles.ManagePermissions)]

        public async Task<Guid> CreateOrUpdateRole(CreateOrUpdateRoleInput input)
        {
            if (input.Role.Id.HasValue)
            {
                return await UpdateRoleAsync(input);
            }
            else
            {
                return await CreateRoleAsync(input);
            }
        }

        protected virtual async Task<Guid> UpdateRoleAsync(CreateOrUpdateRoleInput input)
        {
            Debug.Assert(input.Role.Id != null, "input.Role.Id should be set.");

            var role = await _identityRoleManager.GetByIdAsync(input.Role.Id.Value);

            // todo: Change name
            // todo: We use Display name as name
            // role.ChangeName(input.Role.DisplayName);
            // todo: Default ?
            await _identityRoleManager.SetRoleNameAsync(role, input.Role.DisplayName);
            role.IsDefault = input.Role.IsDefault;
            await _identityRoleManager.UpdateAsync(role);

            var currPerms = await ComputeGrantedPermissionsForRoleAsync(role);
            var toBeRemoved = currPerms.Where(x => !input.GrantedPermissionNames.Contains(x));
            var toBeAdded = input.GrantedPermissionNames.Where(x => !currPerms.Contains(x));
            foreach(var r in toBeRemoved)
            {
                await _permissionManager.SetAsync(r, RolePermissionValueProvider.ProviderName, role.NormalizedName, false);
            }

            foreach(var a in toBeAdded)
            {
                await _permissionManager.SetAsync(a, RolePermissionValueProvider.ProviderName, role.NormalizedName, true);
            }

            return role.Id;
        }

        protected virtual async Task<Guid> CreateRoleAsync(CreateOrUpdateRoleInput input)
        {
            var role = new IdentityRole(_guidGenerator.Create(), input.Role.DisplayName, CurrentTenant.Id);
            role.IsDefault = input.Role.IsDefault;
            // Save
            (await _identityRoleManager.CreateAsync(role)).CheckErrors();
            await CurrentUnitOfWork.SaveChangesAsync(); //It's done to get Id of the role.

            foreach (var a in input.GrantedPermissionNames)
            {
                await _permissionManager.SetAsync(a, RolePermissionValueProvider.ProviderName, role.NormalizedName, true);
            }

            return role.Id;
        }

        protected async Task ComputeGrantedPermissionsForRoleAsync(IdentityRole role, List<string> grantedPermissions) {
            // We exclude those permission for machine clients as well.
            var permDefs = _permissionDefinitionManager.GetPermissions()
                      .Where(x => x.IsEnabled &&
                      x.Name != IdentityPermissions.UserLookup.Default &&
                      x.MultiTenancySide.HasFlag(MultiTenancySides.Tenant));

            foreach (var p in permDefs)
            {
                var isGranted = await _permissionStore.IsGrantedAsync(p.Name, "R", role.NormalizedName);
                if (isGranted)
                {
                    grantedPermissions.Add(p.Name);
                }
            }
        }

        protected async Task<List<string>> ComputeGrantedPermissionsForRoleAsync(IdentityRole role)
        {
            var grantedPermissions = new List<string>();
            await ComputeGrantedPermissionsForRoleAsync(role, grantedPermissions);
            return grantedPermissions;
        }

        // todo: Introduce member permission
        [Authorize(IdentityPermissions.Roles.Default)]
        public async Task<PagedResultDto<NameValueDto<string>>> GetUsersInRoleAsync(FindRoleMembersInput input, CancellationToken token = default)
        {
            var totalCount = await _identityUserRepositoryExt.CountUsersInRoleAsync(input.RoleId, input.Filter, token);
            var items = await _identityUserRepositoryExt
                .GetUsersInRoleAsync(input.RoleId, input.Sorting,
                input.MaxResultCount, input.SkipCount, input.Filter, false, token); // todo: includeDetail?

            return new PagedResultDto<NameValueDto<string>>(
                         totalCount,
                         items.Select(u => new NameValueDto<string>(
                                 $"{u.Name} {u.Surname} ({u.NormalizedEmail})",
                                 u.Id.ToString()
                             )).ToList());
        }


        // todo: Introduce member permission
        [Authorize(IdentityPermissions.Roles.Default)]
        public async Task<PagedResultDto<NameValueDto<string>>> FindUsersAsync(FindRoleMembersInput input, CancellationToken token = default)
        {
            var totalCount = await _identityUserRepositoryExt.CountUsersNotInRoleAsync(input.RoleId, input.Filter, token);
            var items = await _identityUserRepositoryExt
                .GetUsersNotInRoleAsync(input.RoleId, input.Sorting,
                input.MaxResultCount, input.SkipCount, input.Filter, false, token); // includeDetail

            return new PagedResultDto<NameValueDto<string>>(
                         totalCount,
                         items.Select(u => new NameValueDto<string>(
                                 $"{u.Name} {u.Surname} ({u.NormalizedEmail})",
                                 u.Id.ToString()
                             )).ToList());
        }

        [Authorize(IdentityPermissions.Roles.Update)]
        public async Task AddUsersToRoleAsync(UsersToRoleInput input)
        {
            var role = await _identityRoleManager.GetByIdAsync(input.RoleId);
            foreach(var u in input.UserIds)
            {
                var user = await _userManager.GetByIdAsync(u);
                var isMember = await _userManager.IsInRoleAsync(user, role.NormalizedName);
                if (!isMember)
                {
                    (await _userManager.AddToRoleAsync(user, role.NormalizedName)).CheckErrors();
                }
            }
        }

        [Authorize(IdentityPermissions.Roles.Update)]
        public async Task RemoveUsersFromRoleAsync(UsersToRoleInput input)
        {
            var role = await _identityRoleManager.GetByIdAsync(input.RoleId);
            foreach (var u in input.UserIds)
            {
                var user = await _userManager.GetByIdAsync(u);
                var isMember = await _userManager.IsInRoleAsync(user, role.NormalizedName);
                if (isMember)
                {
                    (await _userManager.RemoveFromRoleAsync(user, role.NormalizedName)).CheckErrors();
                }
            }
        }

        [Authorize(IdentityPermissions.Roles.Delete)]
        public async Task DeleteRoleAsync(Guid id)
        {
            var role = await _identityRoleManager.GetByIdAsync(id);

            var totalCount  = await _identityUserRepositoryExt.CountUsersInRoleAsync(role.Id);
            var skipCount = 0;
            while (skipCount < totalCount)
            {
                // Delete all users first 
                var items = await _identityUserRepositoryExt
                    .GetUsersInRoleAsync(role.Id, skipCount: skipCount, maxResultCount: 100);
                skipCount = skipCount + items.Count;

                foreach (var item in items)
                {
                    var user = await _userManager.GetByIdAsync(item.Id);
                    (await _userManager.RemoveFromRoleAsync(user, role.NormalizedName)).CheckErrors();
                }
            }

            await _identityRoleManager.DeleteAsync(role);
        }
    }
}
