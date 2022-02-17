using PolpAbp.Framework;
using System;
using System.Security.Claims;
using Volo.Abp.MultiTenancy;
using Volo.Abp.Security.Claims;
using Volo.Abp.Users;

namespace PolpAbp.ZeroAdaptors
{
    /// <summary>
    /// A helper for establishing the admin as the currently authenticated user.
    /// Also the tenant is properly set.
    /// </summary>
    public abstract class ZeroAdaptorsApplicationTestBase4Admin : ZeroAdaptorsApplicationTestBase, IDisposable
    {
        protected readonly ICurrentTenant CurrentTenant;
        protected readonly ICurrentUser CurrentUser;
        protected readonly ICurrentPrincipalAccessor CurrentPrincipalAccessor;

        public ZeroAdaptorsApplicationTestBase4Admin()
        {
            CurrentTenant = GetRequiredService<ICurrentTenant>();
            CurrentUser = GetRequiredService<ICurrentUser>();
            CurrentPrincipalAccessor = GetRequiredService<ICurrentPrincipalAccessor>();

            var newPrincipal = new ClaimsPrincipal(
            new ClaimsIdentity(
                new Claim[]
                {
                    new Claim(AbpClaimTypes.UserId, FrameworkTestConsts.AdminId.ToString()),
                    new Claim(AbpClaimTypes.TenantId, FrameworkTestConsts.TenantId.ToString()),
                    new Claim(AbpClaimTypes.UserName, "admin")
                }
            ));

            CurrentTenant.Change(FrameworkTestConsts.TenantId);
            CurrentPrincipalAccessor.Change(newPrincipal);
        }
    }
}
