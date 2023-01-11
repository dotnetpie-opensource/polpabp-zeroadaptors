using PolpAbp.Framework;
using Volo.Abp.Application;
using Volo.Abp.Authorization;
using Volo.Abp.BackgroundJobs;
using Volo.Abp.Domain;
using Volo.Abp.Identity;
using Volo.Abp.Localization;
using Volo.Abp.Modularity;
using Volo.Abp.TenantManagement.Localization;
using Volo.Abp.VirtualFileSystem;

namespace PolpAbp.ZeroAdaptors
{
    [DependsOn(
        typeof(AbpDddDomainModule),
        typeof(AbpLocalizationAbstractionsModule), // localization
        typeof(AbpDddApplicationContractsModule),
        typeof(AbpIdentityDomainModule),
        typeof(AbpIdentityDomainSharedModule),
        typeof(AbpIdentityApplicationContractsModule),
        typeof(AbpAuthorizationModule),
        typeof(AbpBackgroundJobsAbstractionsModule),
        typeof(PolpAbpFrameworkApplicationContractsModule),
        typeof(PolpAbpZeroAdaptorsCoreSharedModule)
    )]
    public class PolpAbpZeroAdaptorsApplicationContactsModule : AbpModule
    {
        public override void ConfigureServices(ServiceConfigurationContext context)
        {
            Configure<AbpVirtualFileSystemOptions>(options =>
            {
                options.FileSets.AddEmbedded<PolpAbpZeroAdaptorsApplicationContactsModule>("PolpApb.ZeroAdaptors");
            });

            Configure<AbpLocalizationOptions>(options =>
            {
                options.Resources
                    .Get<AbpTenantManagementResource>()
                    .AddVirtualJson("/Localization/PolpAbp/ZeroAdaptors/MultiTenancy");
            });

        }
    }
}
