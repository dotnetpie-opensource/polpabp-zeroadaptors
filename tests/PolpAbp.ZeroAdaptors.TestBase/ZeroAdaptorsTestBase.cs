using PolpAbp.Framework;
using Volo.Abp.Modularity;

namespace PolpAbp.ZeroAdaptors
{
    public class ZeroAdaptorsTestBase<TStartupModule> : FrameworkTestBase<TStartupModule>
        where TStartupModule : IAbpModule
    {
    }
}
