using PolpAbp.Framework;
using Volo.Abp.Account;
using Volo.Abp.AutoMapper;
using Volo.Abp.Emailing;
using Volo.Abp.Identity;
using Volo.Abp.Ldap;
using Volo.Abp.Modularity;
using Volo.Abp.PermissionManagement;
using Volo.Abp.PermissionManagement.Identity;
using Volo.Abp.SettingManagement;
using Volo.Abp.Sms;
using Volo.Abp.TenantManagement;
using Volo.Abp.TextTemplating.Scriban;
using Volo.Abp.UI.Navigation;

namespace PolpAbp.ZeroAdaptors
{
    [DependsOn(
        typeof(AbpTenantManagementDomainModule),
        typeof(AbpPermissionManagementDomainModule),
        typeof(AbpPermissionManagementDomainIdentityModule),
        typeof(AbpIdentityDomainModule),
        typeof(AbpAccountApplicationContractsModule),
        typeof(AbpTextTemplatingScribanModule),
        typeof(AbpEmailingModule),
        typeof(AbpSmsModule),
        typeof(AbpLdapModule),
        typeof(AbpSettingManagementApplicationModule),
        typeof(AbpUiNavigationModule),
        typeof(PolpAbpFrameworkApplicationContractsModule),
        typeof(PolpAbpFrameworkApplicationModule),
        typeof(PolpAbpFrameworkAbpExtensionsModule),
        typeof(PolpAbpFrameworkAbpExtensionsIdentityModule),
        typeof(PolpAbpFrameworkAbpExtensionsSettingsModule),
        typeof(PolpAbpZeroAdaptorsCoreSharedModule),
        typeof(PolpAbpZeroAdaptorsDomainModule),
        typeof(PolpAbpZeroAdaptorsApplicationContactsModule),
        // Verification code
        typeof(EasyAbp.Abp.VerificationCode.AbpVerificationCodeModule)
    )]
    public class PolpAbpZeroAdaptorsApplicationModule : AbpModule
    {
        public override void ConfigureServices(ServiceConfigurationContext context)
        {
            Configure<AbpAutoMapperOptions>(options =>
            {
                options.AddMaps<PolpAbpZeroAdaptorsApplicationModule>();
            });
        }
    }
}
