using PolpAbp.Framework.BackgroundJobs;
using PolpAbp.ZeroAdaptors.Authorization.Users.Dto;
using Volo.Abp.BackgroundJobs;

namespace PolpAbp.ZeroAdaptors.BackgroundJobs
{
    [BackgroundJobName(nameof(ResetIndividualUserPasswordArgs))]
    public class ResetIndividualUserPasswordArgs : IndividualUserJobArgs
    {
        public ResetUserPasswordDto Payload { get; set; }
    }
}
