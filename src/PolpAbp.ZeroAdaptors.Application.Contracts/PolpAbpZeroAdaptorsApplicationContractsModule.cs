using PolpAbp.Framework;
using Volo.Abp.Account.Localization;
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
        typeof(PolpAbpFrameworkApplicationContractsModule),
        typeof(PolpAbpZeroAdaptorsCoreSharedModule),
        typeof(AbpDddApplicationContractsModule),
        typeof(AbpDddDomainModule),
        typeof(AbpAuthorizationModule),
        typeof(AbpIdentityDomainModule),
        typeof(AbpIdentityApplicationContractsModule),
        typeof(AbpIdentityDomainSharedModule),
        typeof(AbpLocalizationAbstractionsModule), // localization
        typeof(AbpBackgroundJobsAbstractionsModule)
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
                    .AddVirtualJson("/Localization/MultiTenancy/Resources");
            });

        }
    }
}
