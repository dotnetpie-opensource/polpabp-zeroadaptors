using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace PolpAbp.ZeroAdaptors.Authorization.Users.Dto
{
    public class CreateOrUpdateUserInput
    {
        [Required]
        public UserEditDto User { get; set; }

        public string[] AssignedRoleNames { get; set; }

        public bool SendActivationEmail { get; set; }

        public bool SetRandomPassword { get; set; }

        public List<Guid> OrganizationUnits { get; set; }

        public CreateOrUpdateUserInput()
        {
            OrganizationUnits = new List<Guid>();
            AssignedRoleNames = new string[0];
        }
    }
}
