using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using PolpAbp.ZeroAdaptors.Authorization.Users.Dto;
using Volo.Abp;
using Volo.Abp.Application.Dtos;

namespace PolpAbp.ZeroAdaptors.Authorization.Users
{
    [RemoteService(false)]
    public class UserLinkAppService : ZeroAdaptorsAppService, IUserLinkAppService
    {
        public Task<PagedResultDto<LinkedUserDto>> GetLinkedUsers(GetLinkedUsersInput input)
        {
            return Task.FromResult(new PagedResultDto<LinkedUserDto>(0, new List<LinkedUserDto>()));
        }

        public Task<ListResultDto<LinkedUserDto>> GetRecentlyUsedLinkedUsers()
        {
            return Task.FromResult(new ListResultDto<LinkedUserDto>());
        }

        public Task LinkToUser(LinkToUserInput linkToUserInput)
        {
            throw new NotImplementedException();
        }

        public Task UnlinkUser(UnlinkUserInput input)
        {
            throw new NotImplementedException();
        }
    }
}
