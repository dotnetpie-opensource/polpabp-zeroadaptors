using PolpAbp.Framework;
using Volo.Abp.Modularity;

namespace PolpAbp.ZeroAdaptors
{
    [DependsOn(
        typeof(FrameworkTestBaseModule),
        typeof(PolpAbpZeroAdaptorsDomainModule)
        )]
    public class PolpAbpZeroAdaptorsTestBaseModule : AbpModule
    {
    }
}
