using PolpAbp.Framework;
using Volo.Abp.Modularity;

namespace PolpAbp.ZeroAdaptors
{
    [DependsOn(
        typeof(PolpAbpFrameworkTestBaseModule),
        typeof(PolpAbpZeroAdaptorsDomainModule)
        )]
    public class PolpAbpZeroAdaptorsTestBaseModule : AbpModule
    {
    }
}
