using PolpAbp.Framework;
using Volo.Abp.Account;
using Volo.Abp.AutoMapper;
using Volo.Abp.Emailing;
using Volo.Abp.Identity;
using Volo.Abp.Modularity;
using Volo.Abp.PermissionManagement;
using Volo.Abp.PermissionManagement.Identity;
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
        typeof(AbpUiNavigationModule),
        typeof(PolpAbpFrameworkApplicationContractsModule),
        typeof(PolpAbpFrameworkApplicationModule),
        typeof(PolpAbpZeroAdaptorsCoreSharedModule),
        typeof(PolpAbpZeroAdaptorsDomainModule),
        typeof(PolpAbpZeroAdaptorsApplicationContactsModule)
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
