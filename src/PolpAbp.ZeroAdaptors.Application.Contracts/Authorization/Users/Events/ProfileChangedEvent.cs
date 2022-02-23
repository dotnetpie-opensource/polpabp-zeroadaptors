using System;
using System.Collections.Generic;

namespace PolpAbp.ZeroAdaptors.Authorization.Users.Events
{
    public class ProfileChangedEvent
    {
        public Guid? TenantId { get; set; }
        public Guid UserId { get; set; }
        public Guid? OperatorId { get; set; }
        public List<string> ChangedFields { get; set; }
        public bool IsNew { get; set; }
        public bool SendActivationEmail { get; set; }

        public ProfileChangedEvent()
        {
            ChangedFields = new List<string>();
        }
    }
}
