using System;
namespace PolpAbp.ZeroAdaptors.Configuration.Clients.Dto
{
	public class AbpUserTimingConfigDto
	{
		public AbpUserTimeZoneConfigDto TimeZoneInfo
		{
			get;
			set;
		}

		public AbpUserTimingConfigDto()
        {
			TimeZoneInfo = new AbpUserTimeZoneConfigDto();
        }
	}

	public class AbpUserTimeZoneConfigDto
	{
		public AbpUserWindowsTimeZoneConfigDto Windows
		{
			get;
			set;
		}

		public AbpUserIanaTimeZoneConfigDto Iana
		{
			get;
			set;
		}

		public AbpUserTimeZoneConfigDto()
        {
			Windows = new AbpUserWindowsTimeZoneConfigDto();
			Iana = new AbpUserIanaTimeZoneConfigDto();
        }
	}

	public class AbpUserWindowsTimeZoneConfigDto
	{
		public string TimeZoneId
		{
			get;
			set;
		}

		public double BaseUtcOffsetInMilliseconds
		{
			get;
			set;
		}

		public double CurrentUtcOffsetInMilliseconds
		{
			get;
			set;
		}

		public bool IsDaylightSavingTimeNow
		{
			get;
			set;
		}
	}

	public class AbpUserIanaTimeZoneConfigDto
	{
		public string TimeZoneId
		{
			get;
			set;
		}
	}

}
