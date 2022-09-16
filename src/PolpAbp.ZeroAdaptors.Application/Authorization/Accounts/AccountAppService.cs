using System;
using System.Threading.Tasks;
using PolpAbp.Framework.Emailing.Account;
using PolpAbp.ZeroAdaptors.Authorization.Accounts.Dto;
using Volo.Abp;
using Volo.Abp.TenantManagement;
using IUnderlyingAccountAppService = Volo.Abp.Account.IAccountAppService;

namespace PolpAbp.ZeroAdaptors.Authorization.Accounts
{
    [RemoteService(false)]
    public class AccountAppService : ZeroAdaptorsAppService, IAccountAppService
    {
        private readonly ITenantRepository _tenantRepository;
        private readonly IUnderlyingAccountAppService _underlyingAccountApp;
        private readonly IFrameworkAccountEmailer _accountEmailer;

        public AccountAppService(ITenantRepository tenantRepository,
            IUnderlyingAccountAppService underlyingAccountApp,
            IFrameworkAccountEmailer accountEmailer)
        {
            _tenantRepository = tenantRepository;
            _underlyingAccountApp = underlyingAccountApp;
            _accountEmailer = accountEmailer;
        }

        public Task ActivateEmail(ActivateEmailInput input)
        {
            throw new NotImplementedException();
        }

        public Task<ImpersonateOutput> BackToImpersonator()
        {
            throw new NotImplementedException();
        }

        public Task<ImpersonateOutput> Impersonate(ImpersonateInput input)
        {
            throw new NotImplementedException();
        }

        public async Task<IsTenantAvailableOutput> IsTenantAvailable(IsTenantAvailableInput input)
        {
            var tenant = await _tenantRepository.FindByNameAsync(input.TenancyName);
            if (tenant == null)
            {
                return new IsTenantAvailableOutput(TenantAvailabilityState.NotFound);
            }

            /* TODO: in active
            if (!tenant.IsActive)
            {
                return new IsTenantAvailableOutput(TenantAvailabilityState.InActive);
            } */

            return new IsTenantAvailableOutput(TenantAvailabilityState.Available, tenant.Id); // TODO: Fix URL
        }

        public Task<RegisterOutput> Register(RegisterInput input)
        {
            throw new NotImplementedException();
        }

        public Task<ResetPasswordOutput> ResetPassword(ResetPasswordInput input)
        {
            throw new NotImplementedException();
        }

        public Task<Guid?> ResolveTenantId(ResolveTenantIdInput input)
        {
            throw new NotImplementedException();
        }

        public Task SendEmailActivationLink(SendEmailActivationLinkInput input)
        {
            throw new NotImplementedException();
        }

        public async Task SendPasswordResetCode(SendPasswordResetCodeInput input)
        {
            await _underlyingAccountApp.SendPasswordResetCodeAsync(new Volo.Abp.Account.SendPasswordResetCodeDto
            {
                Email = input.EmailAddress,
                AppName = "MVC"
            });
        }

        public Task<SwitchToLinkedAccountOutput> SwitchToLinkedAccount(SwitchToLinkedAccountInput input)
        {
            throw new NotImplementedException();
        }
    }
}
