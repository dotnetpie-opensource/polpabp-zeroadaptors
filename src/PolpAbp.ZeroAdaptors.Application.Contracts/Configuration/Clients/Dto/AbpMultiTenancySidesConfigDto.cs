using System;
using Volo.Abp.MultiTenancy;

namespace PolpAbp.ZeroAdaptors.Configuration.Clients.Dto
{
	public class AbpMultiTenancySidesConfigDto
	{
		public MultiTenancySides Host
		{
			get;
			private set;
		}

		public MultiTenancySides Tenant
		{
			get;
			private set;
		}

		public AbpMultiTenancySidesConfigDto() { }
	}

}
