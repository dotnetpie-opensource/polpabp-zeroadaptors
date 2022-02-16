using System.ComponentModel.DataAnnotations;
using System;

namespace PolpAbp.ZeroAdaptors.Authorization.Accounts.Dto
{
    public class ImpersonateInput
    {
        public Guid? TenantId { get; set; }

        [Range(1, long.MaxValue)]
        public long UserId { get; set; }
    }
}