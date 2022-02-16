using AutoMapper;
using PolpAbp.ZeroAdaptors.Organizations.Dto;
using PolpAbp.ZeroAdaptors.Sessions.Dto;
using System;
using System.Collections.Generic;
using System.Text;
using Volo.Abp.Identity;
using Volo.Abp.TenantManagement;

namespace PolpAbp.ZeroAdaptors
{
    public class ZeroAdaptorsApplicationAutoMapperProfile : Profile
    {
        public ZeroAdaptorsApplicationAutoMapperProfile()
        {
            /* You can configure your AutoMapper mapping configuration here.
             * Alternatively, you can split your mapping configurations
             * into multiple profile classes for a better organization. */

            // Tenant
            CreateMap<Tenant, TenantLoginInfoDto>();

            // User 
            CreateMap<IdentityUser, UserLoginInfoDto>();

            // Tenant 

            //OrganizationUnit
            CreateMap<OrganizationUnit, OrganizationUnitDto>()
                .ForMember(dto => dto.MemberCount, options => options.Ignore())
                .ForMember(dto => dto.RoleCount, options => options.Ignore());

            CreateMap<IdentityUser, OrganizationUnitUserListDto>()
                .ForMember(dto => dto.AddedTime, options => options.Ignore()) // We have no way to get added time yet. todo:
                .ForMember(dto => dto.ProfilePictureId, options => options.Ignore());

            CreateMap<IdentityRole, OrganizationUnitRoleListDto>()
                .ForMember(dto => dto.DisplayName, options => options.MapFrom(src => src.NormalizedName))
                .ForMember(dto => dto.AddedTime, options => options.Ignore()); // We have no way to get added time yet.
        }
    }
}
