using System;
using Volo.Abp.Modularity;

namespace PolpAbp.ZeroAdaptors
{
    [DependsOn(
        typeof(PolpAbpZeroAdaptorsCoreSharedModule)
    )]
    public class PolpAbpZeroAdaptorsCoreModule : AbpModule
    {
    }
}
