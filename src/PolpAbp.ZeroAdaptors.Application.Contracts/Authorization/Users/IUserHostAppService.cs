using PolpAbp.ZeroAdaptors.Authorization.Users.Dto;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Volo.Abp.Application.Services;

namespace PolpAbp.ZeroAdaptors.Authorization.Users
{
    public interface IUserHostAppService : IApplicationService
    {
        /// <summary>
        /// Resets the password for the given user.
        /// </summary>
        /// <param name="tenantId">Tenant ID</param>
        /// <param name="userId">User ID</param>
        /// <param name="input">Reset</param>
        /// <returns>Task</returns>
        Task ResetUserPasswordAsync(Guid? tenantId, Guid userId, ResetUserPasswordDto input);
    }
}
