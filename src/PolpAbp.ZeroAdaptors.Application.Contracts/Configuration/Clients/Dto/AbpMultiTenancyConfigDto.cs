using System;
namespace PolpAbp.ZeroAdaptors.Configuration.Clients.Dto
{
	public class AbpMultiTenancyConfigDto
	{
		public bool IsEnabled
		{
			get;
			set;
		}

		public bool IgnoreFeatureCheckForHostUsers
		{
			get;
			set;
		}

		public AbpMultiTenancySidesConfigDto Sides
		{
			get;
			private set;
		}

		public AbpMultiTenancyConfigDto() {
			Sides = new AbpMultiTenancySidesConfigDto();
		}
	}

}
