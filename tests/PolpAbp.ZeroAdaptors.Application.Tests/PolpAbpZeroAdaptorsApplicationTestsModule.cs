using System;
using System.Collections.Generic;
using System.Text;
using Volo.Abp.Modularity;
using PolpAbp.Framework;

namespace PolpAbp.ZeroAdaptors
{
    [DependsOn(
        typeof(FrameworkDomainTestsModule),
        typeof(PolpAbpZeroAdaptorsTestBaseModule),
        typeof(PolpAbpZeroAdaptorsApplicationModule)
        )]
    public class PolpAbpZeroAdaptorsApplicationTestsModule : AbpModule
    {
    }
}
