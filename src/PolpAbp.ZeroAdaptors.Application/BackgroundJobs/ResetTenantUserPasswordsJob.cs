using PolpAbp.Framework.Identity;
using System.Linq;
using System.Threading.Tasks;
using Volo.Abp.BackgroundJobs;
using Volo.Abp.DependencyInjection;
using Volo.Abp.Identity;
using Volo.Abp.MultiTenancy;

namespace PolpAbp.ZeroAdaptors.BackgroundJobs
{
    public class ResetTenantUserPasswordsJob : AsyncBackgroundJob<ResetTenantUserPasswordsArgs>, ITransientDependency
    {
        private readonly ICurrentTenant _currentTenant;
        private readonly IIdentityUserRepository _identityUserRepository;
        private readonly IBackgroundJobManager _backgroundJobManager;

        public ResetTenantUserPasswordsJob(ICurrentTenant tenant,
            IIdentityUserRepository identityUserRepository,
            IBackgroundJobManager backgroundJobManager)
        {
            _currentTenant = tenant;
            _identityUserRepository = identityUserRepository;
            _backgroundJobManager = backgroundJobManager;
        }

        public override async Task ExecuteAsync(ResetTenantUserPasswordsArgs args)
        {
            var iterator = new TenantMemberIterator(_currentTenant, _identityUserRepository);
            await iterator.RunAsync(args.TenantId, async (items) =>
            {
                // Exclude the external user
                var candidates = items.Where(x => !args.ExcludedUsers.Contains(x.Id) && !x.IsExternal);

                foreach (var candidate in candidates)
                {
                    await _backgroundJobManager.EnqueueAsync(new ResetIndividualUserPasswordArgs
                    {
                        TenantId = args.TenantId,
                        UserId = candidate.Id,
                        Payload = args.Payload 
                    });
                }
            });

        }
    }
}
