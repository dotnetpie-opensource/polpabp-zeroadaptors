using System;
using System.Collections.Generic;
using System.Text;
using Volo.Abp.MultiTenancy;
using PolpAbp.Framework;
using System.Threading.Tasks;
using Xunit;
using Volo.Abp.Identity;
using Volo.Abp.Users;
using Volo.Abp.Security.Claims;
using System.Security.Claims;
using Volo.Abp.Data;

namespace PolpAbp.ZeroAdaptors.Authorization.Users.Profile
{
    public class ProfileAppServiceTests : ZeroAdaptorsApplicationTestBase
    {
        private readonly IProfileAppService _profileAppService;
        private readonly IdentityUserManager _identityUserManager;
        private readonly ICurrentTenant _currentTenant;
        private readonly ICurrentUser _currentUser;
        private readonly ICurrentPrincipalAccessor _currentPrincipalAccessor;
        private readonly IDataFilter _dataFilter;

        public ProfileAppServiceTests()
        {
            _profileAppService = GetRequiredService<IProfileAppService>();
            _identityUserManager = GetRequiredService<IdentityUserManager>();
            _currentTenant = GetRequiredService<ICurrentTenant>();
            _currentUser = GetRequiredService<ICurrentUser>();
            _currentPrincipalAccessor = GetRequiredService<ICurrentPrincipalAccessor>();
            _dataFilter = GetRequiredService<IDataFilter>();

        }

        [Fact]
        public async Task CanChangePasswordAsync()
        {
            var newPrincipal = new ClaimsPrincipal(
            new ClaimsIdentity(
                new Claim[]
                {
                    new Claim(AbpClaimTypes.UserId, FrameworkTestConsts.AdminId.ToString()),
                    new Claim(AbpClaimTypes.TenantId, FrameworkTestConsts.TenantId.ToString()),
                    new Claim(AbpClaimTypes.UserName, "admin"),
                }
            ));
            using (_currentTenant.Change(FrameworkTestConsts.TenantId))
            using (_currentPrincipalAccessor.Change(newPrincipal))
            {

                var newPass = "1234Asdfzxc!@";
                await _profileAppService.ChangePasswordAsync(new Dto.ChangePasswordInput
                {
                    CurrentPassword = FrameworkTestConsts.AdminPass,
                    NewPassword = newPass
                });

                var adminUser = await _identityUserManager.GetByIdAsync(FrameworkTestConsts.AdminId);

                // Next verify its password 
                var ret = await _identityUserManager.CheckPasswordAsync(adminUser, newPass);

                Assert.True(ret);
            }
        }
    }
}
