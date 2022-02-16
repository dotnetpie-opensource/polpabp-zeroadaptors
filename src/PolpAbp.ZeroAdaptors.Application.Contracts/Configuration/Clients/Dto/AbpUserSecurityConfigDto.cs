using System;
namespace PolpAbp.ZeroAdaptors.Configuration.Clients.Dto
{
	public class AbpUserSecurityConfigDto
	{
		public AbpUserAntiForgeryConfigDto AntiForgery
		{
			get;
			set;
		}

		public AbpUserSecurityConfigDto()
        {
			AntiForgery = new AbpUserAntiForgeryConfigDto();
        }
	}

	public class AbpUserAntiForgeryConfigDto
	{
		public string TokenCookieName
		{
			get;
			set;
		}

		public string TokenHeaderName
		{
			get;
			set;
		}
	}

}
