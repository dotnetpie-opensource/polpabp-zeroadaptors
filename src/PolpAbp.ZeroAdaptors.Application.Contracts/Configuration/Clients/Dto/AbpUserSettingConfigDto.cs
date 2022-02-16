using System;
using System.Collections.Generic;

namespace PolpAbp.ZeroAdaptors.Configuration.Clients.Dto
{
	public class AbpUserSettingConfigDto
	{
		public Dictionary<string, string> Values
		{
			get;
			set;
		}

		public AbpUserSettingConfigDto()
        {
			Values = new Dictionary<string, string>();
        }
	}

}
