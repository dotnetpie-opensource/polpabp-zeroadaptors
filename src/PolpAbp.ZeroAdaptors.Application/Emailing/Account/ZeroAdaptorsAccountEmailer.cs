using Microsoft.Extensions.Localization;
using System.Text.Encodings.Web;
using System.Threading.Tasks;
using Volo.Abp.Account.Localization;
using Volo.Abp.Data;
using Volo.Abp.DependencyInjection;
using Volo.Abp.Emailing;
using Volo.Abp.Identity;
using Volo.Abp.MultiTenancy;
using Volo.Abp.TenantManagement;
using Volo.Abp.TextTemplating;
using Volo.Abp.UI.Navigation.Urls;

namespace PolpAbp.ZeroAdaptors.Emailing.Account
{
    public class ZeroAdaptorsAccountEmailer : IZeroAdaptorsAccountEmailer, ITransientDependency
    {
        private readonly ITemplateRenderer _templateRenderer;
        private readonly IEmailSender _emailSender;
        protected IStringLocalizer<AccountResource> StringLocalizer { get; }
        private readonly IAppUrlProvider _appUrlProvider;
        private readonly ICurrentTenant _currentTenant;
        private readonly IdentityUserManager _userManager;
        private readonly IDataFilter _dataFilter;
        private readonly ITenantRepository _tenantRepository;

        public ZeroAdaptorsAccountEmailer(
            IEmailSender emailSender,
            ITemplateRenderer templateRenderer,
            IStringLocalizer<AccountResource> stringLocalizer,
            IAppUrlProvider appUrlProvider,
            ICurrentTenant currentTenant,
            IdentityUserManager userManager,
            IDataFilter dataFilter,
            ITenantRepository tenantRepository)
        {
            _emailSender = emailSender;
            StringLocalizer = stringLocalizer;
            _appUrlProvider = appUrlProvider;
            _currentTenant = currentTenant;
            _templateRenderer = templateRenderer;
            _userManager = userManager;
            _dataFilter = dataFilter;
            _tenantRepository = tenantRepository;
        }


        public async Task SendEmailActivationLinkAsync(string email)
        {
            using (_dataFilter.Disable<IMultiTenant>())
            {

                var user = await _userManager.FindByEmailAsync(email);
                var tenant = await _tenantRepository.FindAsync(user.TenantId.Value);
                var token = await _userManager.GenerateEmailConfirmationTokenAsync(user);

                var url = await _appUrlProvider.GetUrlAsync("MVC", ZeroAdaptorsUrlNames.EmailActivation);

                var link = $"{url}?userId={user.Id}&tenantId={user.TenantId}&confirmationCode={UrlEncoder.Default.Encode(token)}";

                var emailContent = await _templateRenderer.RenderAsync(
                    Templates.AccountEmailTemplates.EmailActivationtLink,
                    new
                    {
                        link = link,
                        tenancy = tenant.Name
                    }
                );

                await _emailSender.SendAsync(
                    user.Email,
                    StringLocalizer["EmailActivation_Subject"],
                    emailContent
                );
            }
        }
    }
}
