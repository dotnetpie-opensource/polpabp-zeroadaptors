using System;
using System.Collections.Generic;

namespace PolpAbp.ZeroAdaptors.Configuration.Clients.Dto
{
	public class AbpUserAuthConfigDto
	{
		public Dictionary<string, string> AllPermissions
		{
			get;
			set;
		}

		public Dictionary<string, string> GrantedPermissions
		{
			get;
			set;
		}

		public AbpUserAuthConfigDto()
        {
			AllPermissions = new Dictionary<string, string>();
			GrantedPermissions = new Dictionary<string, string>();
        }
	}

}
