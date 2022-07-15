using System;
using System.Collections.Generic;
using Volo.Abp.Data;

namespace PolpAbp.ZeroAdaptors.Authorization.Users.Events
{
    public class PasswordChangedEvent : IHasExtraProperties
    {
        public Guid? TenantId { get; set; }
        public Guid UserId { get; set; }
        public Guid? OperatorId { get; set; }
        public string NewPassword { get; set; }

        public ExtraPropertyDictionary ExtraProperties { get; set; }

        public PasswordChangedEvent()
        {
            ExtraProperties = new ExtraPropertyDictionary();
        }
    }
}
