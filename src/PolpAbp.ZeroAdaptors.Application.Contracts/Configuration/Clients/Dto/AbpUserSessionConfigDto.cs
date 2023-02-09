using System;
using Volo.Abp.MultiTenancy;

namespace PolpAbp.ZeroAdaptors.Configuration.Clients.Dto
{
	public class AbpUserSessionConfigDto
	{
		public string UserId
		{
			get;
			set;
		}

		public string TenantId
		{
			get;
			set;
		}

		public string ImpersonatorUserId
		{
			get;
			set;
		}

		public string ImpersonatorTenantId
		{
			get;
			set;
		}

		public MultiTenancySides MultiTenancySide
		{
			get;
			set;
		}

		public AbpUserSessionConfigDto()
        {
			UserId = string.Empty;
			TenantId = string.Empty;
			ImpersonatorTenantId = string.Empty;
			ImpersonatorUserId = string.Empty;
        }
	}
}
