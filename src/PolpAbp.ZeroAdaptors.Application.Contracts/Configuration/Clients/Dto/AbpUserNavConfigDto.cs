using System;
using System.Collections.Generic;

namespace PolpAbp.ZeroAdaptors.Configuration.Clients.Dto
{
	public class AbpUserNavConfigDto
	{
		public Dictionary<string, UserMenu> Menus
		{
			get;
			set;
		}

		public AbpUserNavConfigDto()
        {
			Menus = new Dictionary<string, UserMenu>();
        }
	}
}
