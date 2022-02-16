using System;
using System.Collections.Generic;
using System.Text;
using Volo.Abp.Emailing.Templates;
using Volo.Abp.Localization;
using Volo.Abp.TextTemplating;
using PolpAbp.ZeroAdaptors.Localization.Account;

namespace PolpAbp.ZeroAdaptors.Emailing.Account.Templates
{
    public class AccountEmailTemplateDefinitionProvider : TemplateDefinitionProvider
    {
        public override void Define(ITemplateDefinitionContext context)
        {
            context.Add(
                           new TemplateDefinition(
                               AccountEmailTemplates.EmailActivationtLink,
                               displayName: LocalizableString.Create<ZeroAdaptorsAccountResource>($"TextTemplate:{AccountEmailTemplates.EmailActivationtLink}"),
                               layout: StandardEmailTemplates.Layout,
                               localizationResource: typeof(ZeroAdaptorsAccountResource)
                           ).WithVirtualFilePath("/Emailing/Account/Templates/EmailActivationLink.tpl", true)
                       );
        }
    }
}
