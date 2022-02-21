using System;

namespace PolpAbp.ZeroAdaptors.Authorization.Users.Events
{
    public class PasswordChangedEvent
    {
        public Guid? TenantId { get; set; }
        public Guid UserId { get; set; }
        public Guid? OperatorId { get; set; }
        public string NewPassword { get; set; }
    }
}
