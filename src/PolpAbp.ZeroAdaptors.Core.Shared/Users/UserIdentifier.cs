using System;

namespace PolpAbp.ZeroAdaptors.Users
{
    //
    // Summary:
    //     Used to identify a user.
    public class UserIdentifier : IUserIdentifier
    {
        //
        // Summary:
        //     Initializes a new instance of the Abp.UserIdentifier class.
        //
        // Parameters:
        //   tenantId:
        //     Tenant Id of the user.
        //
        //   userId:
        //     Id of the user.
        public UserIdentifier(Guid? tenantId, string userId) { }
        //
        // Summary:
        //     Initializes a new instance of the Abp.UserIdentifier class.
        protected UserIdentifier() { }

        //
        // Summary:
        //     Tenant Id of the user. Can be null for host users in a multi tenant application.
        public Guid? TenantId { get; protected set; }
        //
        // Summary:
        //     Id of the user.
        public string UserId { get; protected set; }

        //
        // Summary:
        //     Parses given string and creates a new Abp.UserIdentifier object.
        //
        // Parameters:
        //   userIdentifierString:
        //     Should be formatted one of the followings: - "userId@tenantId". Ex: "42@3" (for
        //     tenant users). - "userId". Ex: 1 (for host users)
        public static UserIdentifier Parse(string userIdentifierString)
        {
            var pieces = userIdentifierString.Split("@");
            if (pieces.Length == 2)
            {
                return new UserIdentifier(new Guid(pieces[0]), pieces[1]);
            }

            if (pieces.Length == 1)
            {
                return new UserIdentifier(null, pieces[0]);
            }

            return new UserIdentifier();
        }
        public override bool Equals(object obj)
        {
            return this == obj as UserIdentifier;
        }
        public override int GetHashCode()
        {
            // TODO: Fix 
            return ToUserIdentifierString().GetHashCode(); 
        }

        //public override string ToString();
        //
        // Summary:
        //     Creates a string represents this Abp.UserIdentifier instance. Formatted one of
        //     the followings: - "userId@tenantId". Ex: "42@3" (for tenant users). - "userId".
        //     Ex: 1 (for host users) Returning string can be used in Abp.UserIdentifier.Parse(System.String)
        //     method to re-create identical Abp.UserIdentifier object.
        public string ToUserIdentifierString()
        {
            return $"{UserId}@{TenantId}";
        }

        public static bool operator ==(UserIdentifier left, UserIdentifier right)
        {
            if (left.TenantId.HasValue && right.TenantId.HasValue)
            {
                return left.TenantId.Value == right.TenantId.Value && string.Equals(left.UserId, right.UserId, StringComparison.OrdinalIgnoreCase);
            }

            if (!left.TenantId.HasValue && !right.TenantId.HasValue)
            {
                return string.Equals(left.UserId, right.UserId, StringComparison.OrdinalIgnoreCase);
            }

            return false;
        }
        public static bool operator !=(UserIdentifier left, UserIdentifier right)
        {
            return !(left == right);
        }
    }
}
