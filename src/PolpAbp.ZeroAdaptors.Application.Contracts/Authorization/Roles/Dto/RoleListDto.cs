﻿using System;
using Volo.Abp.Application.Dtos;
using Volo.Abp.Auditing;

namespace PolpAbp.ZeroAdaptors.Authorization.Roles.Dto
{
    public class RoleListDto : EntityDto, IHasCreationTime
    {
        public Guid Id { get; set; }
        public string Name { get; set; }

        public string DisplayName { get; set; }

        public bool IsStatic { get; set; }

        public bool IsDefault { get; set; }

        public bool IsPublic { get; set; }

        public DateTime CreationTime { get; set; }
    }
}