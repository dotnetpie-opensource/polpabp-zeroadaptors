using System;
using System.Collections.Generic;

namespace PolpAbp.ZeroAdaptors.Configuration.Clients.Dto
{
    public class AbpUserConfigurationDto
    {
		//  public ApplicationLocalizationConfigurationDto Localization { get; set; }
		public AbpUserLocalizationConfigDto Localization
		{
			get;
			set;
		}

		// public ApplicationAuthConfigurationDto Auth { get; set; }
		public AbpUserAuthConfigDto Auth
		{
			get;
			set;
		}

		// public ApplicationSettingConfigurationDto Setting { get; set; }
		public AbpUserSettingConfigDto Setting
		{
			get;
			set;
		}

		// Missing
		// public CurrentUserDto CurrentUser { get; set; }

		// public ApplicationFeatureConfigurationDto Features { get; set; }
		public AbpUserFeatureConfigDto Features
		{
			get;
			set;
		}

		// public MultiTenancyInfoDto MultiTenancy { get; set; }
		public AbpMultiTenancyConfigDto MultiTenancy
		{
			get;
			set;
		}

		// Missing
		// public CurrentTenantDto CurrentTenant { get; set; }


		// No corresponding
		// Maybe get the values from CurrentTeant and CurrentUser
		public AbpUserSessionConfigDto Session
		{
			get;
			set;
		}

		// No corresponding 
		public AbpUserNavConfigDto Nav
		{
			get;
			set;
		}

		// public TimingDto Timing { get; set; }
		public AbpUserTimingConfigDto Timing
		{
			get;
			set;
		}

		// public ClockDto Clock { get; set; }
		public AbpUserClockConfigDto Clock
		{
			get;
			set;
		}

		// No corresponding
		public AbpUserSecurityConfigDto Security
		{
			get;
			set;
		}

		public Dictionary<string, object> Custom
		{
			get;
			set;
		}

		public AbpUserConfigurationDto()
        {
			Localization = new AbpUserLocalizationConfigDto();
			Auth = new AbpUserAuthConfigDto();
			Setting = new AbpUserSettingConfigDto();
			MultiTenancy = new AbpMultiTenancyConfigDto();
			Session = new AbpUserSessionConfigDto();
			Nav = new AbpUserNavConfigDto();
			Timing = new AbpUserTimingConfigDto();
			Clock = new AbpUserClockConfigDto();
			Security = new AbpUserSecurityConfigDto();
			Custom = new Dictionary<string, object>();
			Features = new AbpUserFeatureConfigDto();

        }

        public string CrossDomain { get; set; }
    }
}
