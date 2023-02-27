using PolpAbp.Framework;
using PolpAbp.Framework.Common.Dto;
using PolpAbp.ZeroAdaptors.Authorization.Roles.Dto;
using System;
using System.Threading;
using System.Threading.Tasks;
using Volo.Abp.Application.Dtos;
using Volo.Abp.Application.Services;

namespace PolpAbp.ZeroAdaptors.Authorization.Roles
{
    /// <summary>
    /// Application service that is used by 'role management' page.
    /// </summary>
    public interface IRoleAppService : IApplicationService
    {
        Task<ListResultDto<RoleListDto>> GetRoles(GetRolesInput input);

        Task<GetRoleForEditOutput> GetRoleForEdit(NullableIdDto<Guid> input);

        Task<Guid> CreateOrUpdateRole(CreateOrUpdateRoleInput input);
        Task<PagedResultDto<NameValueDto<string>>> GetUsersInRoleAsync(FindRoleMembersInput input, CancellationToken token = default);
        Task<PagedResultDto<NameValueDto<string>>> FindUsersAsync(FindRoleMembersInput input, CancellationToken token = default);
        Task RemoveUsersFromRoleAsync(UsersToRoleInput input);
        Task AddUsersToRoleAsync(UsersToRoleInput input);
        Task DeleteRoleAsync(Guid id);

        // Task DeleteRole(EntityDto input);
    }
}