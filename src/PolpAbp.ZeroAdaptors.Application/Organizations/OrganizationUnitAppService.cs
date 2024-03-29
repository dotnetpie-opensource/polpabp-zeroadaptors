﻿using Microsoft.AspNetCore.Authorization;
using PolpAbp.Framework.Authorization;
using PolpAbp.ZeroAdaptors.Organizations.Dto;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Volo.Abp;
using Volo.Abp.Application.Dtos;
using Volo.Abp.Guids;
using Volo.Abp.Identity;
using PolpAbp.Framework.Identity;

namespace PolpAbp.ZeroAdaptors.Organizations
{
    [RemoteService(false)]
    public class OrganizationUnitAppService : ZeroAdaptorsAppService, IOrganizationUnitAppService
    {
        private readonly OrganizationUnitManager _organizationUnitManager;
        private readonly IOrganizationUnitRepository _organizationUnitRepository;
        private readonly IdentityUserManager _identityUserManager;
        private readonly IGuidGenerator _guidGenerator;
        private readonly IIdentityUserRepositoryExt _identityUserRepositoryExt;

        public OrganizationUnitAppService(
            OrganizationUnitManager organizationUnitManager,
            IOrganizationUnitRepository organizationUnitRepository,
            IdentityUserManager identityUserManager,
            IGuidGenerator guidGenerator,
            IIdentityUserRepositoryExt identityUserRepositoryExt
            )
        {
            _organizationUnitManager = organizationUnitManager;
            _organizationUnitRepository = organizationUnitRepository;
            _guidGenerator = guidGenerator;
            _identityUserManager = identityUserManager;
            _identityUserRepositoryExt = identityUserRepositoryExt;
        }

        [Authorize(OrganizationUnitPermissions.Default)]
        public async Task<ListResultDto<OrganizationUnitDto>> GetOrganizationUnitsAsync()
        {
            var organizationUnits = await _organizationUnitRepository.GetListAsync();
            var items = new List<OrganizationUnitDto>();
            foreach(var ou in organizationUnits)
            {
                var dto = ObjectMapper.Map<OrganizationUnit, OrganizationUnitDto>(ou);
                dto.MemberCount = await _organizationUnitRepository.GetMembersCountAsync(ou);
                dto.RoleCount = await _organizationUnitRepository.GetRolesCountAsync(ou);

                items.Add(dto);
            }

            var ret = new ListResultDto<OrganizationUnitDto>(items);
            return ret;
        }

        [Authorize(OrganizationUnitPermissions.Default)]
        public async Task<PagedResultDto<OrganizationUnitUserListDto>> GetOrganizationUnitUsersAsync(GetOrganizationUnitUsersInput input)
        {
            var totalCount = await _identityUserRepositoryExt.CountUsersInOrganizationUnitAsync(input.Id, filter: input.Filter);
            var items = await _identityUserRepositoryExt
                .GetUsersInOrganizationUnitAsync(input.Id, sorting: input.Sorting, maxResultCount: input.MaxResultCount, skipCount: input.SkipCount, filter: input.Filter);

            return new PagedResultDto<OrganizationUnitUserListDto>(
                totalCount,
                items.Select(item =>
                {
                    var organizationUnitUserDto = ObjectMapper.Map<IdentityUser, OrganizationUnitUserListDto>(item);
                    organizationUnitUserDto.AddedTime = item.CreationTime;
                    return organizationUnitUserDto;
                }).ToList());
        }

        [Authorize(OrganizationUnitPermissions.Default)]
        public async Task<PagedResultDto<OrganizationUnitRoleListDto>> GetOrganizationUnitRolesAsync(GetOrganizationUnitRolesInput input)
        {

            var ou = await _organizationUnitRepository.GetAsync(input.Id);
            var totalCount = await _organizationUnitRepository.GetRolesCountAsync(ou);
            var items = await _organizationUnitRepository
                .GetRolesAsync(ou, sorting: input.Sorting, maxResultCount: input.MaxResultCount, skipCount: input.SkipCount);

            return new PagedResultDto<OrganizationUnitRoleListDto>(
                totalCount,
                items.Select(item =>
                {
                    var organizationUnitRoleDto = ObjectMapper.Map<IdentityRole, OrganizationUnitRoleListDto>(item);
                    organizationUnitRoleDto.AddedTime = DateTime.Now;
                    return organizationUnitRoleDto;
                }).ToList());
        } 

        [Authorize(OrganizationUnitPermissions.ManageTree)]
        public async Task<OrganizationUnitDto> CreateOrganizationUnitAsync(CreateOrganizationUnitInput input)
        {
            var organizationUnit = new OrganizationUnit(_guidGenerator.Create(),input.DisplayName, input.ParentId, CurrentTenant.Id);

            await _organizationUnitManager.CreateAsync(organizationUnit);

            return ObjectMapper.Map<OrganizationUnit, OrganizationUnitDto>(organizationUnit);
        }

        [Authorize(OrganizationUnitPermissions.ManageTree)]
        public async Task<OrganizationUnitDto> UpdateOrganizationUnitAsync(UpdateOrganizationUnitInput input)
        {
            var organizationUnit = await _organizationUnitRepository.GetAsync(input.Id);

            organizationUnit.DisplayName = input.DisplayName;

            await _organizationUnitManager.UpdateAsync(organizationUnit);

            return ObjectMapper.Map<OrganizationUnit, OrganizationUnitDto>(organizationUnit);
        }

        [Authorize(OrganizationUnitPermissions.ManageTree)]
        public async Task<OrganizationUnitDto> MoveOrganizationUnitAsync(MoveOrganizationUnitInput input)
        {
            await _organizationUnitManager.MoveAsync(input.Id, input.NewParentId);
            var entity = await _organizationUnitRepository.GetAsync(input.Id);
            return ObjectMapper.Map<OrganizationUnit, OrganizationUnitDto>(entity);
        }

        [Authorize(OrganizationUnitPermissions.ManageTree)]
        public async Task DeleteOrganizationUnitAsync(Guid input)
        {
            await _organizationUnitManager.DeleteAsync(input);
        }

        [Authorize(OrganizationUnitPermissions.ManageMembers)]
        public async Task RemoveUserFromOrganizationUnitAsync(UserToOrganizationUnitInput input)
        {
            await _identityUserManager.RemoveFromOrganizationUnitAsync(input.UserId, input.OrganizationUnitId);
        }

        [Authorize(OrganizationUnitPermissions.ManageRoles)]
        public async Task RemoveRoleFromOrganizationUnitAsync(RoleToOrganizationUnitInput input)
        {
            await _organizationUnitManager.RemoveRoleFromOrganizationUnitAsync(input.RoleId, input.OrganizationUnitId);
        }

        [Authorize(OrganizationUnitPermissions.ManageMembers)]
        public async Task AddUsersToOrganizationUnitAsync(UsersToOrganizationUnitInput input)
        {
            foreach (var userId in input.UserIds)
            {
                await _identityUserManager.AddToOrganizationUnitAsync(userId, input.OrganizationUnitId);
            }
        }

        [Authorize(OrganizationUnitPermissions.ManageRoles)]
        public async Task AddRolesToOrganizationUnitAsync(RolesToOrganizationUnitInput input)
        {
            foreach (var roleId in input.RoleIds)
            {
                await _organizationUnitManager.AddRoleToOrganizationUnitAsync(roleId, input.OrganizationUnitId);
            }
        } 

        [Authorize(OrganizationUnitPermissions.ManageMembers)]
        public async Task<PagedResultDto<NameValueDto<string>>> FindUsersAsync(FindOrganizationUnitUsersInput input)
        {
            var totalCount = await _identityUserRepositoryExt.CountUsersNotInOrganizationUnitAsync(input.OrganizationUnitId, input.Filter);
            var items = await _identityUserRepositoryExt
                .GetUsersNotInOrganizationUnitAsync(input.OrganizationUnitId, sorting: input.Sorting, maxResultCount: input.MaxResultCount, skipCount: input.SkipCount, filter: input.Filter);

            return new PagedResultDto<NameValueDto<string>>(
                totalCount,
                items.Select(u => new NameValueDto<string>(
                        $"{u.Name} {u.Surname} ({u.NormalizedEmail})",
                        u.Id.ToString()
                    )).ToList());

        }

        [Authorize(OrganizationUnitPermissions.ManageRoles)]
        public async Task<PagedResultDto<NameValueDto<string>>> FindRolesAsync(FindOrganizationUnitRolesInput input)
        {
            var ou = await _organizationUnitRepository.GetAsync(input.OrganizationUnitId);
            var totalCount = await _organizationUnitRepository.GetUnaddedRolesCountAsync(ou, filter: input.Filter);
            var items = await _organizationUnitRepository
                .GetUnaddedRolesAsync(ou, sorting: input.Sorting, maxResultCount: input.MaxResultCount, skipCount: input.SkipCount, filter: input.Filter);

            return new PagedResultDto<NameValueDto<string>>(
                totalCount,
                items.Select(u => new NameValueDto<string>(
                        u.NormalizedName, 
                        u.Id.ToString()
                    )).ToList());
        }
    }
}