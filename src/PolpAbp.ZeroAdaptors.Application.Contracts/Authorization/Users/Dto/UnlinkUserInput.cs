using System;

namespace PolpAbp.ZeroAdaptors.Authorization.Users.Dto
{
    public class UnlinkUserInput
    {
        public Guid? TenantId { get; set; }

        public string UserId { get; set; }

        //public UserIdentifier ToUserIdentifier()
        //{
        //    return new UserIdentifier(TenantId, UserId);
        //}
    }
}