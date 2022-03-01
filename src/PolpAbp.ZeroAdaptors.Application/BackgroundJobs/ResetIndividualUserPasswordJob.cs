using PolpAbp.ZeroAdaptors.Authorization.Users;
using System.Threading.Tasks;
using Volo.Abp.BackgroundJobs;
using Volo.Abp.DependencyInjection;

namespace PolpAbp.ZeroAdaptors.BackgroundJobs
{
    public class ResetIndividualUserPasswordJob : AsyncBackgroundJob<ResetIndividualUserPasswordArgs>, ITransientDependency
    {
        private readonly IUserHostAppService _userHostAppService;

        public ResetIndividualUserPasswordJob(
            IUserHostAppService userHostAppService)
        {
            _userHostAppService = userHostAppService;
        }

        public async override Task ExecuteAsync(ResetIndividualUserPasswordArgs args)
        {
            await _userHostAppService.ResetUserPasswordAsync(args.TenantId, args.UserId, args.Payload, args.OperatorId);
        }
    }
}
