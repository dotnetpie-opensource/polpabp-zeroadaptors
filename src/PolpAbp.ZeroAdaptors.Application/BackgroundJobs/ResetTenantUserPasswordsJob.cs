﻿using PolpAbp.Framework.Identity;
using System;
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
            var counter = 0;
            await iterator.RunAsync(args.TenantId, async (items) =>
            {
                // Exclude the external user
                var candidates = items.Where(x => !args.ExcludedUsers.Contains(x.Id) && !x.IsExternal);

                foreach (var candidate in candidates)
                {
                    counter++;
                    var delay = System.Math.Ceiling(counter / 5.0);
                    await _backgroundJobManager.EnqueueAsync(new ResetIndividualUserPasswordArgs
                    {
                        TenantId = args.TenantId,
                        UserId = candidate.Id,
                        Payload = args.Payload,
                        OperatorId = args.OperatorId
                    }, delay: TimeSpan.FromSeconds(3 * delay));
                }
            });

        }
    }
}
