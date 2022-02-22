using PolpAbp.ZeroAdaptors.Authorization.Users.Dto;
using System;
using System.Threading.Tasks;
using Volo.Abp.Application.Services;

namespace PolpAbp.ZeroAdaptors.Authorization.Users
{
    public interface IUserAppService : IApplicationService
    {
        /// <summary>
        /// Creates a user with the given information.
        /// </summary>
        /// <param name="input">User information</param>
        /// <returns>Task with the newly created user ID</returns>
        Task<Guid> CreateUserAsync(CreateOrUpdateUserInput input);
        /// <summary>
        /// Gets the necessary information for creating a user.
        /// </summary>
        /// <returns>Task with the user information</returns>
        Task<GetUserForEditOutput> GetUserForCreateAsync();
        /// <summary>
        /// Gets the ncessary information for editing an existing user.
        /// </summary>
        /// <param name="input">User Id</param>
        /// <returns>Task with the user information</returns>
        Task<GetUserForEditOutput> GetUserForEditAsync(Guid input);
        /// <summary>
        /// Gets the permission information for the given user.
        /// </summary>
        /// <param name="input">User ID</param>
        /// <returns>Task with the permissions</returns>
        Task<GetUserPermissionsForEditOutput> GetUserPermissionsForEditAsync(Guid input);

        /// <summary>
        /// Resets the password for the given user.
        /// </summary>
        /// <param name="id">User ID</param>
        /// <param name="input">Reset password request</param>
        /// <param name="runValidator">Flag to run the password validator</param>
        /// <returns>Task</returns>
        Task ResetUserPasswordAsync(Guid id, ResetUserPasswordDto input, bool runValidator);

        /// <summary>
        /// Resets the permission information for the given user.
        /// </summary>
        /// <param name="id">User ID</param>
        /// <returns>Task</returns>
        Task ResetUserSpecificPermissionsAsync(Guid id);
        /// <summary>
        /// Updates a user with the given information.
        /// </summary>
        /// <param name="input">User information</param>
        /// <returns>Task</returns>
        Task UpdateUserAsync(CreateOrUpdateUserInput input);
        /// <summary>
        /// Updates a user's permission with the given information.
        /// </summary>
        /// <param name="input">Information</param>
        /// <returns>Task</returns>
        Task UpdateUserPermissionsAsync(UpdateUserPermissionsInput input);
    }
}
