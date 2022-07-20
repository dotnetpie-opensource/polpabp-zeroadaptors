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
using Volo.Abp.UI.Navigation.Urls;
using Volo.Abp.VirtualFileSystem;

namespace PolpAbp.ZeroAdaptors
{
    [DependsOn(
        typeof(PolpAbpFrameworkApplicationContractsModule),
        typeof(PolpAbpFrameworkApplicationModule),
        typeof(PolpAbpZeroAdaptorsDomainModule),
        typeof(PolpAbpZeroAdaptorsCoreSharedModule),
        typeof(PolpAbpZeroAdaptorsApplicationContactsModule),
        typeof(AbpPermissionManagementDomainModule),
        typeof(AbpTenantManagementDomainModule),
        typeof(AbpIdentityDomainModule),
        typeof(AbpPermissionManagementDomainIdentityModule),
        typeof(AbpAccountApplicationContractsModule),
        typeof(AbpTextTemplatingScribanModule),
        typeof(AbpEmailingModule),
        typeof(AbpUiNavigationModule)
    )]
    public class PolpAbpZeroAdaptorsApplicationModule : AbpModule
    {
        public override void ConfigureServices(ServiceConfigurationContext context)
        {
            Configure<AbpAutoMapperOptions>(options =>
            {
                options.AddMaps<PolpAbpZeroAdaptorsApplicationModule>();
            });

            Configure<AppUrlOptions>(options =>
            {
                options.Applications["MVC"].Urls[ZeroAdaptorsUrlNames.EmailActivation] = "Account/EmailActivation";
            });

            Configure<AbpVirtualFileSystemOptions>(options =>
            {
                options.FileSets.AddEmbedded<PolpAbpZeroAdaptorsApplicationModule>("PolpAbp.ZeroAdaptors");
            });

        }
    }
}
