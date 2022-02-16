using Polpware.ComponentModel.DataAnnotations.Validators;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace PolpAbp.ZeroAdaptors.Authorization.Users.Dto
{
    public class UpdateUserPermissionsInput
    {
        [NotEmpty]
        public Guid Id { get; set; }

        [Required]
        [HasSomeMember(1)]
        public List<string> GrantedPermissionNames { get; set; }
    }
}
