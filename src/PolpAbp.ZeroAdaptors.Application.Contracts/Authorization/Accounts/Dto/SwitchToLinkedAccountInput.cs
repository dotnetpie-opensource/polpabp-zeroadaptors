using System;
using System.ComponentModel.DataAnnotations;

namespace PolpAbp.ZeroAdaptors.Authorization.Accounts.Dto
{
    public class SwitchToLinkedAccountInput
    {
        public Guid? TargetTenantId { get; set; }

        [Range(1, long.MaxValue)]
        public long TargetUserId { get; set; }

        /*
        public UserIdentifier ToUserIdentifier()
        {
            return new UserIdentifier(TargetTenantId, TargetUserId);
        } */
    }
}
