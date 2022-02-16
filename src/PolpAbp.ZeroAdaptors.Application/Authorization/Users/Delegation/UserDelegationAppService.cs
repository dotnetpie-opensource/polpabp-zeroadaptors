using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using PolpAbp.ZeroAdaptors.Authorization.Users.Delegation.Dto;
using Volo.Abp;
using Volo.Abp.Application.Dtos;

namespace PolpAbp.ZeroAdaptors.Authorization.Users.Delegation
{
    [RemoteService(false)]
    public class UserDelegationAppService : ZeroAdaptorsAppService, IUserDelegationAppService
    {

        public Task DelegateNewUser(CreateUserDelegationDto input)
        {
            throw new NotImplementedException();
        }

        public Task<List<UserDelegationDto>> GetActiveUserDelegations()
        {
            return Task.FromResult(new List<UserDelegationDto>());
        }

        public Task<PagedResultDto<UserDelegationDto>> GetDelegatedUsers(GetUserDelegationsInput input)
        {
            throw new NotImplementedException();
        }

        public Task RemoveDelegation(EntityDto<long> input)
        {
            throw new NotImplementedException();
        }
    }
}
