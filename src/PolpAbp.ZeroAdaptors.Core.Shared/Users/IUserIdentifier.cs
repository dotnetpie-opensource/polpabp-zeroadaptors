using System;
using System.Collections.Generic;
using System.Text;

namespace PolpAbp.ZeroAdaptors.Users
{
    //
    // Summary:
    //     Interface to get a user identifier.
    public interface IUserIdentifier
    {
        //
        // Summary:
        //     Tenant Id. Can be null for host users.
        Guid? TenantId { get; }
        //
        // Summary:
        //     Id of the user.
        string UserId { get; }
    }
}
