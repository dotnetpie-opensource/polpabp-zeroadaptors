using System;
using Volo.Abp.Application.Dtos;
using Volo.Abp.Timing;

namespace PolpAbp.ZeroAdaptors.Sessions.Dto
{
    public class TenantLoginInfoDto : EntityDto
    {
        public string TenancyName { get; set; }

        public string Name { get; set; }

        public Guid? LogoId { get; set; }

        public string LogoFileType { get; set; }

        public Guid? CustomCssId { get; set; }

        public DateTime? SubscriptionEndDateUtc { get; set; }

        public bool IsInTrialPeriod { get; set; }

        // TODO: Fix 
        // public SubscriptionPaymentType SubscriptionPaymentType { get; set; }

        public EditionInfoDto Edition { get; set; }

        public DateTime CreationTime { get; set; }

        // TODO: Fix
        // public PaymentPeriodType PaymentPeriodType { get; set; }

        public string SubscriptionDateString { get; set; }

        public string CreationTimeString { get; set; }

        public bool IsInTrial()
        {
            return IsInTrialPeriod;
        }

        public bool SubscriptionIsExpiringSoon(int subscriptionExpireNootifyDayCount)
        {
            if (SubscriptionEndDateUtc.HasValue)
            {
                // TODO: Fix 
                // return Clock.Now.ToUniversalTime().AddDays(subscriptionExpireNootifyDayCount) >= SubscriptionEndDateUtc.Value;
                return false;
            }

            return false;
        }

        public int GetSubscriptionExpiringDayCount()
        {
            if (!SubscriptionEndDateUtc.HasValue)
            {
                return 0;
            }

            // TODO: Fix 
            //return Convert.ToInt32(SubscriptionEndDateUtc.Value.ToUniversalTime().Subtract(Clock.Now.ToUniversalTime()).TotalDays);
            return 0;
        }

        public bool HasRecurringSubscription()
        {
            // TODO: Fix
            //return SubscriptionPaymentType != SubscriptionPaymentType.Manual;
            return false;
        }
    }
}