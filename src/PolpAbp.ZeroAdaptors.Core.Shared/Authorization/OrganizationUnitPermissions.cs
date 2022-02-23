using Volo.Abp.Reflection;

namespace PolpAbp.ZeroAdaptors.Authorization
{
    /// <summary>
    /// Defines the list of permissions applying to organization units.
    /// </summary>
    public static class OrganizationUnitPermissions
    {
        private const string GroupName = "PolpAbp";

        public const string Default = GroupName + ".OrganizationUnits";
        public const string ManageTree = Default + ".ManageOrganizationTree";
        public const string ManageMembers = Default + ".ManageMembers";
        public const string ManageRoles = Default + ".ManageRoles";
        public const string ManagePermissions = Default + "ManagePermissions";

        public static string[] GetAll()
        {
            return ReflectionHelper.GetPublicConstantsRecursively(typeof(OrganizationUnitPermissions));
        }
    }
}