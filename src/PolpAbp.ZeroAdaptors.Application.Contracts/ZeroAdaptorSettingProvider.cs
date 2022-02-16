using System;
using System.Collections.Generic;
using System.Text;
using Volo.Abp.Settings;
using PolpAbp.ZeroAdaptors.Settings;

namespace PolpAbp.ZeroAdaptors
{
    public class ZeroAdaptorSettingProvider : SettingDefinitionProvider
    {
        public override void Define(ISettingDefinitionContext context)
        {
            context.Add(
                new SettingDefinition(LocalizationSettingNames.DefaultLanguage, "en")
            );
        }
    }
}
