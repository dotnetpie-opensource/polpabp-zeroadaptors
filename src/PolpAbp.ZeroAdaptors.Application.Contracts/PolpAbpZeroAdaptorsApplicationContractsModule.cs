using System;
using Volo.Abp.Modularity;
using Volo.Abp.Localization;
using Volo.Abp.VirtualFileSystem;
using Volo.Abp.Identity;
using Volo.Abp.Authorization;
using Volo.Abp.Domain;
using Volo.Abp.Application;
using Volo.Abp.BackgroundJobs;
using PolpAbp.ZeroAdaptors.Localization.Account;
using Volo.Abp.Localization.ExceptionHandling;
using PolpAbp.ZeroAdaptors.Localization.Organizations;
using PolpAbp.ZeroAdaptors.Localization.MultiTenancy;
using PolpAbp.Framework;

namespace PolpAbp.ZeroAdaptors
{
    [DependsOn(
        typeof(PolpAbpFrameworkApplicationContractsModule),
        typeof(PolpAbpFrameworkCoreSharedModule),
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
                    .Add<ZeroAdaptorsAccountResource>("en")
                    .AddVirtualJson("/Localization/Account/Resources");

                options.Resources
                .Add<ZeroAdaptorsOrganizationsResource>("en")
                .AddVirtualJson("/Localization/Organizations/Resources");

                options.Resources
                .Add<ZeroAdaptorsMultiTenancyResources>("en")
                .AddVirtualJson("/Localization/MultiTenancy/Resources");
            });

            Configure<AbpExceptionLocalizationOptions>(options =>
            {
                options.MapCodeNamespace("PolpApb.ZeroAdaptors", typeof(ZeroAdaptorsAccountResource));
                // todo: What to do if we have more than 
                //options.MapCodeNamespace("PolpApb.ZeroAdaptors", typeof(ZeroAdaptorsOrganizationsResource));
                //options.MapCodeNamespace("PolpApb.ZeroAdaptors", typeof(ZeroAdaptorsMultiTenancyResources));

            });
        }
    }
}
