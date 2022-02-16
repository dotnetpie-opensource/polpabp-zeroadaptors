using PolpAbp.ZeroAdaptors.Organizations.Dto;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Volo.Abp.Application.Dtos;
using Volo.Abp.Application.Services;

namespace PolpAbp.ZeroAdaptors.Organizations
{
    public interface IOrganizationUnitAppService : IApplicationService
    {
        Task<ListResultDto<OrganizationUnitDto>> GetOrganizationUnitsAsync();
        Task<OrganizationUnitDto> CreateOrganizationUnitAsync(CreateOrganizationUnitInput input);
        Task<OrganizationUnitDto> UpdateOrganizationUnitAsync(UpdateOrganizationUnitInput input);
        Task DeleteOrganizationUnitAsync(Guid input);
        Task<OrganizationUnitDto> MoveOrganizationUnitAsync(MoveOrganizationUnitInput input);
 
        Task<PagedResultDto<OrganizationUnitUserListDto>> GetOrganizationUnitUsersAsync(GetOrganizationUnitUsersInput input);
        Task<PagedResultDto<OrganizationUnitRoleListDto>> GetOrganizationUnitRolesAsync(GetOrganizationUnitRolesInput input);
        Task<PagedResultDto<NameValueDto<string>>> FindUsersAsync(FindOrganizationUnitUsersInput input);
        Task<PagedResultDto<NameValueDto<string>>> FindRolesAsync(FindOrganizationUnitRolesInput input);


        Task RemoveUserFromOrganizationUnitAsync(UserToOrganizationUnitInput input);

        Task RemoveRoleFromOrganizationUnitAsync(RoleToOrganizationUnitInput input);

        Task AddUsersToOrganizationUnitAsync(UsersToOrganizationUnitInput input);

        Task AddRolesToOrganizationUnitAsync(RolesToOrganizationUnitInput input);
    }
}
