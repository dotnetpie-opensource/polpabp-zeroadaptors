namespace PolpAbp.ZeroAdaptors.Authorization.Permissions
{
    /// <summary>
    /// Provides the permission transformations 
    /// across the application. 
    /// By defining an applicaiton specific implementation, 
    /// the application may have its own expected permission 
    /// names.
    /// The packages include a identity mappping function 
    /// as the default implementation.
    /// </summary>
    public interface IPermissionConvertor
    {
        /// <summary>
        /// Converts the given application-specific permission name 
        /// into a common one from a library
        /// </summary>
        /// <param name="input">Application-specific permission name</param>
        /// <returns>Permission name</returns>
        string Decode(string input);

        /// <summary>
        /// Converts the given permission name into 
        /// an application-specific one.
        /// </summary>
        /// <param name="input">Permission name</param>
        /// <returns>Application-specific permission name</returns>
        string Encode(string input);
    }
}
