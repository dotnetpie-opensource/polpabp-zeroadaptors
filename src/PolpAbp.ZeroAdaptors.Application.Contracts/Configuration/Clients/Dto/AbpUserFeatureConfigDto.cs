using System;
using System.Collections.Generic;

namespace PolpAbp.ZeroAdaptors.Configuration.Clients.Dto
{
	public class AbpUserFeatureConfigDto
	{
		public Dictionary<string, AbpStringValueDto> AllFeatures
		{
			get;
			set;
		}

		public AbpUserFeatureConfigDto()
        {
			AllFeatures = new Dictionary<string, AbpStringValueDto>();
        }
	}

}
