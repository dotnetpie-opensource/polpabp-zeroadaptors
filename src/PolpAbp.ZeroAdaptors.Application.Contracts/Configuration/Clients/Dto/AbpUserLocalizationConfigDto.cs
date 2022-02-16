using System;
using System.Collections.Generic;

namespace PolpAbp.ZeroAdaptors.Configuration.Clients.Dto
{
	public class AbpUserLocalizationConfigDto
	{
		public AbpUserCurrentCultureConfigDto CurrentCulture
		{
			get;
			set;
		}

		public List<LanguageInfo> Languages
		{
			get;
			set;
		}

		public LanguageInfo CurrentLanguage
		{
			get;
			set;
		}

		public List<AbpLocalizationSourceDto> Sources
		{
			get;
			set;
		}

		public Dictionary<string, Dictionary<string, string>> Values
		{
			get;
			set;
		}

		/// <summary>
        /// CurrentLanugage has to be set
        /// </summary>
		public AbpUserLocalizationConfigDto()
        {
			Languages = new List<LanguageInfo>();
			Sources = new List<AbpLocalizationSourceDto>();
			Values = new Dictionary<string, Dictionary<string, string>>();
			CurrentCulture = new AbpUserCurrentCultureConfigDto();
        }
	}
}
