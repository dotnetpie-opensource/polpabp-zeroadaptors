using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using PolpAbp.Framework.Common.Dto;
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
        private readonly IPermissionAppService _permissionAppService;
        private readonly IPermissionDefinitionManager _permissionDefinitionManager;
        private readonly IPermissionStore _permissionStore;
        private readonly IGuidGenerator _guidGenerator;

        public RoleAppService(
            IIdentityRoleAppService identityRoleAppService,
            IdentityRoleManager identityRoleManager,
            IPermissionManager permissionManager,
            IPermissionAppService permissionAppService,
            IPermissionDefinitionManager permissionDefinitionManager,
            IPermissionStore permissionStore,
            IGuidGenerator guidGenerator
            )
        {
            _identityRoleAppService = identityRoleAppService;
            _identityRoleManager = identityRoleManager;
            _permissionManager = permissionManager;
            _permissionAppService = permissionAppService;
            _permissionDefinitionManager = permissionDefinitionManager;
            _permissionStore = permissionStore;
            _guidGenerator = guidGenerator;
        }

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

                await ComputeGrantedPermissionsForRoleAsync(role.Id, grantedPermissions);
            }

            var permissions = permDefs.Select(x => new FlatPermissionDto
            {
                Name = x.Name,
                ParentName = x.Parent?.Name,
                DisplayName = x.DisplayName.Localize(StringLocalizerFactory)
            }).OrderBy(y => y.DisplayName).ToList();

            return new GetRoleForEditOutput
            {
                Role = roleEditDto,
                Permissions =  permissions,
                GrantedPermissionNames = grantedPermissions
            };
        }

        public async Task CreateOrUpdateRole(CreateOrUpdateRoleInput input)
        {
            if (input.Role.Id.HasValue)
            {
                await UpdateRoleAsync(input);
            }
            else
            {
                await CreateRoleAsync(input);
            }
        }

        protected virtual async Task UpdateRoleAsync(CreateOrUpdateRoleInput input)
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

            var currPerms = await ComputeGrantedPermissionsForRoleAsync(input.Role.Id.Value);
            var toBeRemoved = currPerms.Where(x => !input.GrantedPermissionNames.Contains(x));
            var toBeAdded = input.GrantedPermissionNames.Where(x => !currPerms.Contains(x));

            var updatedPermissions = new List<UpdatePermissionDto>();
            foreach(var r in toBeRemoved)
            {
                updatedPermissions.Add(new UpdatePermissionDto
                {
                    IsGranted = false,
                    Name = r
                });
            }

            foreach(var a in toBeAdded)
            {
                updatedPermissions.Add(new UpdatePermissionDto
                {
                    IsGranted = true,
                    Name = a
                });
            }

            await _permissionAppService.UpdateAsync("R", input.Role.Id.Value.ToString(), new UpdatePermissionsDto
            {
                Permissions = updatedPermissions.ToArray()
            });
        }

        protected virtual async Task CreateRoleAsync(CreateOrUpdateRoleInput input)
        {
            var role = new IdentityRole(_guidGenerator.Create(), input.Role.DisplayName, CurrentTenant.Id);
            role.IsDefault = input.Role.IsDefault;
            // Save
            (await _identityRoleManager.CreateAsync(role)).CheckErrors();
            await CurrentUnitOfWork.SaveChangesAsync(); //It's done to get Id of the role.

            var updatedPermissions = new List<UpdatePermissionDto>();

            foreach (var a in input.GrantedPermissionNames)
            {
                updatedPermissions.Add(new UpdatePermissionDto
                {
                    IsGranted = true,
                    Name = a
                });
            }

            await _permissionAppService.UpdateAsync("R", input.Role.Id.Value.ToString(), new UpdatePermissionsDto
            {
                Permissions = updatedPermissions.ToArray()
            });
        }

        protected async Task ComputeGrantedPermissionsForRoleAsync(Guid roleId, List<string> grantedPermissions) {
            // We exclude those permission for machine clients as well.
            var permDefs = _permissionDefinitionManager.GetPermissions()
                      .Where(x => x.IsEnabled &&
                      x.Name != IdentityPermissions.UserLookup.Default &&
                      x.MultiTenancySide.HasFlag(MultiTenancySides.Tenant));

            foreach (var p in permDefs)
            {
                var isGranted = await _permissionStore.IsGrantedAsync(p.Name, "R", roleId.ToString());
                if (isGranted)
                {
                    grantedPermissions.Add(p.Name);
                }
            }
        }

        protected async Task<List<string>> ComputeGrantedPermissionsForRoleAsync(Guid roleId)
        {
            var grantedPermissions = new List<string>();
            await ComputeGrantedPermissionsForRoleAsync(roleId, grantedPermissions);
            return grantedPermissions;
        }

    }
}
