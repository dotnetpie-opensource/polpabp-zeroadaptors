using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using PolpAbp.ZeroAdaptors.Authorization.Users.Dto;
using Volo.Abp.Application.Services;

namespace PolpAbp.ZeroAdaptors.Authorization.Users
{
    public interface IUserAppService : IApplicationService
    {
        Task<Guid> CreateUserAsync(CreateOrUpdateUserInput input);
        Task<GetUserForEditOutput> GetUserForEditAsync();
        Task<GetUserForEditOutput> GetUserForEditAsync(Guid input);
        Task<GetUserPermissionsForEditOutput> GetUserPermissionsForEditAsync(Guid input);
        Task ResetUserSpecificPermissionsAsync(Guid id);
        Task UpdateUserAsync(CreateOrUpdateUserInput input);
        Task UpdateUserPermissionsAsync(UpdateUserPermissionsInput input);
    }
}
