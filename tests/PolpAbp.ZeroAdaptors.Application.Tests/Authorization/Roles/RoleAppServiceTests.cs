using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;
using System.Threading.Tasks;
using Volo.Abp.Identity;
using Xunit;

namespace PolpAbp.ZeroAdaptors.Authorization.Roles
{
    public class RoleAppServiceTests : ZeroAdaptorsApplicationTestBase4Admin
    {
        private readonly IRoleAppService _roleAppService;
        private readonly IdentityRoleManager _identityRoleManager;

        public RoleAppServiceTests()
        {
            _roleAppService = GetRequiredService<IRoleAppService>();
            _identityRoleManager = GetRequiredService<IdentityRoleManager>();  
        }

        [Fact]
        public async Task CanCreateRoleWithoutPermissionsAsync()
        {
            await _roleAppService.CreateOrUpdateRole(new Dto.CreateOrUpdateRoleInput
            {
                GrantedPermissionNames = new List<string>(),
                Role = new Dto.RoleEditDto
                {
                    DisplayName = "testwithoutperm",
                    IsDefault = true
                }
            });

            var isExist = await _identityRoleManager.RoleExistsAsync("testwithoutperm");

            Assert.True(isExist);
        }

        [Fact]
        public async Task CanCreateRoleWithPermissionsAsync()
        {
            await _roleAppService.CreateOrUpdateRole(new Dto.CreateOrUpdateRoleInput
            {
                GrantedPermissionNames = new List<string>()
                {
                    IdentityPermissions.Users.Create
                },
                Role = new Dto.RoleEditDto
                {
                    DisplayName = "testwithperm",
                    IsDefault = true
                }
            });

            var isExist = await _identityRoleManager.RoleExistsAsync("testwithperm");

            Assert.True(isExist);

            var roles = await _roleAppService.GetRoles(new Dto.GetRolesInput());
            Assert.True(roles.Items.Count > 0);
            var role = roles.Items.FirstOrDefault(x => string.Equals(x.Name, "testwithperm", StringComparison.OrdinalIgnoreCase));

            Assert.NotNull(role);

            // Permissions 
            var info = await _roleAppService.GetRoleForEdit(new Framework.Common.Dto.NullableIdDto<Guid>
            {
                Id = role.Id
            });

            Assert.NotNull(info.Permissions);
            Assert.True(info.GrantedPermissionNames.Count == 1);    
        }

        [Fact]
        public async Task CanUpdateRoleAsync()
        {
            await _roleAppService.CreateOrUpdateRole(new Dto.CreateOrUpdateRoleInput
            {
                GrantedPermissionNames = new List<string>()
                {
                    IdentityPermissions.Users.Create
                },
                Role = new Dto.RoleEditDto
                {
                    DisplayName = "testwithupdate",
                    IsDefault = true
                }
            });

            var isExist = await _identityRoleManager.RoleExistsAsync("testwithupdate");

            Assert.True(isExist);

            var roles = await _roleAppService.GetRoles(new Dto.GetRolesInput());
            Assert.True(roles.Items.Count > 0);
            var role = roles.Items.FirstOrDefault(x => string.Equals(x.Name, "testwithupdate", StringComparison.OrdinalIgnoreCase));

            Assert.NotNull(role);

            // Permissions 
            var info = await _roleAppService.GetRoleForEdit(new Framework.Common.Dto.NullableIdDto<Guid>
            {
                Id = role.Id
            });

            Assert.NotNull(info.Permissions);
            Assert.True(info.GrantedPermissionNames.Count == 1);

            await _roleAppService.CreateOrUpdateRole(new Dto.CreateOrUpdateRoleInput
            {
                GrantedPermissionNames = new List<string>(),
                Role = new Dto.RoleEditDto
                {
                    Id = role.Id,
                    DisplayName = "testwithupdate455"
                }
            });

            var isnewExist = await _identityRoleManager.RoleExistsAsync("testwithupdate455");
            Assert.True(isExist);

            var newInfo = await _roleAppService.GetRoleForEdit(new Framework.Common.Dto.NullableIdDto<Guid>
            {
                Id = role.Id
            });

            Assert.NotNull(newInfo.Permissions);
            Assert.True(newInfo.GrantedPermissionNames.Count == 0);
        }

        [Fact]
        public async Task CanGetUsersInRoleAsync()
        {
            var roles = await _roleAppService.GetRoles(new Dto.GetRolesInput());
            var role = roles.Items.FirstOrDefault(x => string.Equals(x.Name, "admin", StringComparison.OrdinalIgnoreCase));

            var users = await _roleAppService.GetUsersInRoleAsync(new Dto.FindRoleMembersInput
            {
                RoleId = role.Id,
                MaxResultCount = 100,
                SkipCount = 0
            });

            Assert.NotNull(users);
            Assert.True(users.TotalCount > 0);
        }


        [Fact]
        public async Task CanFindUsers()
        {
            var roles = await _roleAppService.GetRoles(new Dto.GetRolesInput());
            var role = roles.Items.FirstOrDefault(x => string.Equals(x.Name, "admin", StringComparison.OrdinalIgnoreCase));

            var users = await _roleAppService.FindUsersAsync(new Dto.FindRoleMembersInput
            {
                RoleId = role.Id,
                MaxResultCount = 100,
                SkipCount = 0
            });

            Assert.NotNull(users);
            Assert.True(users.TotalCount > 0);
        }
    }
}
