using System;
using System.Collections.Generic;
using Volo.Abp.Data;

namespace PolpAbp.ZeroAdaptors.Authorization.Users.Events
{
    public class ProfileChangedEvent : IHasExtraProperties
    {
        public Guid? TenantId { get; set; }
        public Guid UserId { get; set; }
        public Guid? OperatorId { get; set; }
        public List<string> ChangedFields { get; set; }
        public bool IsNew { get; set; }
        public bool SendActivationEmail { get; set; }

        public ExtraPropertyDictionary ExtraProperties {get;set; }

        public ProfileChangedEvent()
        {
            ChangedFields = new List<string>();
            ExtraProperties = new ExtraPropertyDictionary();
        }
    }
}
