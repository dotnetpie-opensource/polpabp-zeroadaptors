using PolpAbp.Framework.BackgroundJobs;
using PolpAbp.ZeroAdaptors.Authorization.Users.Dto;
using System;
using System.Collections.Generic;
using Volo.Abp.BackgroundJobs;

namespace PolpAbp.ZeroAdaptors.BackgroundJobs
{
    [BackgroundJobName(nameof(ResetTenantUserPasswordsArgs))]
    public class ResetTenantUserPasswordsArgs : IndividualTenantJobArgs
    {
        public ResetUserPasswordDto Payload { get; set; }

        public List<Guid> ExcludedUsers { get; set; }
    }
}
