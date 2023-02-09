using System.Threading.Tasks;
using PolpAbp.Framework.Security;
using PolpAbp.ZeroAdaptors.Configuration.Dto;
using PolpAbp.ZeroAdaptors.Configuration.Host.Dto;
using PolpAbp.ZeroAdaptors.Configuration.Tenants.Dto;
using Volo.Abp.Application.Services;

namespace PolpAbp.ZeroAdaptors.Configuration.Tenants
{
    public interface ITenantSettingsAppService : IApplicationService
    {
        Task<TenantEmailSettingsEditDto> GetEmailSettingsAsync();
        Task<LdapSettingsEditDto> GetLdapSettingsAsync();
        Task<PasswordComplexitySetting> GetPasswordComplexitySettingsAsync();
        Task<TwoFactorLoginSettingsEditDto> GetTwoFactorLoginSettingsAsync();
        Task<UserLockOutSettingsEditDto> GetUserLockOutSettingsAsync();
        Task<TenantUserManagementSettingsEditDto> GetUserManagementSettingsAsync();
        Task ResetEmailSettingsAsync();
        Task ResetLdapSettingsAsync();
        Task ResetPasswordComplexitySettingsAsync();
        Task ResetTwoFactorLoginSettingsAsync();
        Task ResetUserLockOutSettingsAsync();
        Task ResetUserManagementSessionTimeOutSettingsAsync();
        Task ResetUserManagementSettingsAsync();
        Task SendTestEmailAsync(SendTestEmailInput input);
        Task UpdatePasswordComplexitySettingsAsync(PasswordComplexitySetting settings);
        Task UpdateTwoFactorLoginSettingsAsync(TwoFactorLoginSettingsEditDto settings);
        Task UpdateUserLockOutSettingsAsync(UserLockOutSettingsEditDto settings);
        Task UpdateUserManagementSessionTimeOutSettingsAsync(SessionTimeOutSettingsEditDto settings);
        Task UpdateUserManagementSettingsAsync(TenantUserManagementSettingsEditDto settings);
    }
}
