using System.Threading.Tasks;
using Volo.Abp.Application.Services;
using PolpAbp.ZeroAdaptors.Authorization.Roles.Dto;
using Volo.Abp.Application.Dtos;
using PolpAbp.Framework.Common.Dto;
using System;

namespace PolpAbp.ZeroAdaptors.Authorization.Roles
{
    /// <summary>
    /// Application service that is used by 'role management' page.
    /// </summary>
    public interface IRoleAppService : IApplicationService
    {
        Task<ListResultDto<RoleListDto>> GetRoles(GetRolesInput input);

        Task<GetRoleForEditOutput> GetRoleForEdit(NullableIdDto<Guid> input);

        Task CreateOrUpdateRole(CreateOrUpdateRoleInput input);

        // Task DeleteRole(EntityDto input);
    }
}