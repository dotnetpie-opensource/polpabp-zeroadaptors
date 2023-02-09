using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.Configuration;
using PolpAbp.Framework.Security;
using PolpAbp.Framework.Settings;
using PolpAbp.ZeroAdaptors.Configuration.Dto;
using PolpAbp.ZeroAdaptors.Configuration.Host.Dto;
using PolpAbp.ZeroAdaptors.Configuration.Tenants.Dto;
using System;
using System.Threading.Tasks;
using Volo.Abp;
using Volo.Abp.Account.Settings;
using Volo.Abp.Emailing;
using Volo.Abp.Identity.Settings;
using Volo.Abp.Ldap;
using Volo.Abp.SettingManagement;
using Volo.Abp.Settings;

namespace PolpAbp.ZeroAdaptors.Configuration.Tenants
{
    [Authorize]
    public class TenantSettingsAppService : ZeroAdaptorsAppService, ITenantSettingsAppService
    {
        protected readonly ISettingConvertor SettingConvertor;
        protected readonly ISettingManager SettingManager;
        protected readonly IConfiguration Configuration;
        protected readonly IEmailSender EmailSender;


        public TenantSettingsAppService(ISettingConvertor settingConvertor,
            ISettingManager settingManager,
            IConfiguration configuration,
            IEmailSender emailSender)
        {
            SettingConvertor = settingConvertor;
            SettingManager = settingManager;
            Configuration = configuration;
            EmailSender = emailSender;
        }

        public async Task SendTestEmailAsync(SendTestEmailInput input)
        {
            try
            {
                await EmailSender.SendAsync(
                    input.EmailAddress,
                    L["TestEmail_Subject"],
                    L["TestEmail_Body"]
                );
            }
            catch (Exception e)
            {
                throw new UserFriendlyException("An error was encountered while sending an email. " + e.Message);
            }
        }

        public async Task<LdapSettingsEditDto> GetLdapSettingsAsync()
        {
            // Note that we cannot use the fallback settings, as those can be changed. 
            // Therefore, the tenant-level settings must be self-contained and do not depend on the fallback settings.
            return new LdapSettingsEditDto
            {
                IsModuleEnabled = false,
                IsEnabled = false, // todo: Introduce a setting
                Domain = await SettingManager.GetOrNullForCurrentTenantAsync(LdapSettingNames.Domain, fallback: false),
                UserName = await SettingManager.GetOrNullForCurrentTenantAsync(LdapSettingNames.UserName, fallback: false),
                Password = await SettingManager.GetOrNullForCurrentTenantAsync(LdapSettingNames.Password, fallback: false),
            };
        }

        public async Task ResetLdapSettingsAsync()
        {
            await SettingManager.SetForCurrentTenantAsync(LdapSettingNames.Domain, null);
            await SettingManager.SetForCurrentTenantAsync(LdapSettingNames.UserName, null);
            await SettingManager.SetForCurrentTenantAsync(LdapSettingNames.Password, null);
        }

        public async Task<TenantEmailSettingsEditDto> GetEmailSettingsAsync()
        {
            var useHostDefaultEmailSettings = 
                await SettingProvider.GetAsync<bool>(FrameworkSettings.TenantManagement.EmailSettings.UseHostDefault);

            if (useHostDefaultEmailSettings)
            {
                return new TenantEmailSettingsEditDto
                {
                    UseHostDefaultEmailSettings = true
                };
            }

            return new TenantEmailSettingsEditDto
            {
                UseHostDefaultEmailSettings = false,
                // Note that we cannot use the fallback settings, as those can be changed. 
                // Therefore, the tenant-level settings must be self-contained and do not depend on the fallback settings.
                DefaultFromAddress = await SettingManager.GetOrNullForCurrentTenantAsync(EmailSettingNames.DefaultFromAddress, fallback: false),
                DefaultFromDisplayName = await SettingManager.GetOrNullForCurrentTenantAsync(EmailSettingNames.DefaultFromDisplayName, fallback: false),
                SmtpHost = await SettingManager.GetOrNullForCurrentTenantAsync(EmailSettingNames.Smtp.Host, fallback: false),
                SmtpPort = await SettingManager.GetForCurrentTenantAsync<int>(EmailSettingNames.Smtp.Port, fallback: false),
                SmtpUserName = await SettingManager.GetOrNullForCurrentTenantAsync(EmailSettingNames.Smtp.UserName, fallback: false),
                SmtpPassword = await SettingManager.GetOrNullForCurrentTenantAsync(EmailSettingNames.Smtp.Password, fallback: false), // tod: Need decrypt?
                SmtpDomain = await SettingManager.GetOrNullForCurrentTenantAsync(EmailSettingNames.Smtp.Domain, fallback: false),
                SmtpEnableSsl = await SettingManager.GetForCurrentTenantAsync<bool>(EmailSettingNames.Smtp.EnableSsl, fallback: false),
                SmtpUseDefaultCredentials = await SettingManager.GetForCurrentTenantAsync<bool>(EmailSettingNames.Smtp.UseDefaultCredentials, fallback: false)
            };
        }

        public async Task UpdateEmailSettingsAsync(TenantEmailSettingsEditDto input)
        {
            if (!input.UseHostDefaultEmailSettings)
            {
                // Note that we cannot use the fallback settings, as those can be changed. 
                // Therefore, the tenant-level settings must be self-contained and do not depend on the fallback settings.
                await SettingManager.SetForCurrentTenantAsync(EmailSettingNames.DefaultFromAddress, input.DefaultFromAddress, forceToSet: true);
                await SettingManager.SetForCurrentTenantAsync(EmailSettingNames.DefaultFromDisplayName, input.DefaultFromDisplayName, forceToSet: true);
                await SettingManager.SetForCurrentTenantAsync(EmailSettingNames.Smtp.Host, input.SmtpHost, forceToSet: true);
                await SettingManager.SetForCurrentTenantAsync(EmailSettingNames.Smtp.Port, input.SmtpPort.ToString(), forceToSet: true);
                await SettingManager.SetForCurrentTenantAsync(EmailSettingNames.Smtp.UserName, input.SmtpUserName, forceToSet: true);
                await SettingManager.SetForCurrentTenantAsync(EmailSettingNames.Smtp.Password, input.SmtpPassword, forceToSet: true); // tod: Need decrypt?
                await SettingManager.SetForCurrentTenantAsync(EmailSettingNames.Smtp.Domain, input.SmtpDomain, forceToSet: true);
                await SettingManager.SetForCurrentTenantAsync(EmailSettingNames.Smtp.EnableSsl, input.SmtpEnableSsl.ToString(), forceToSet: true);
                await SettingManager.SetForCurrentTenantAsync(EmailSettingNames.Smtp.UseDefaultCredentials, input.SmtpUseDefaultCredentials.ToString(), forceToSet: true);
            }
            // On purpose we have the flag set the last step. 
            // Therefore, we can tolerate some error. E.g., the above fails then the following too. 
            await SettingManager
                .SetForCurrentTenantAsync(FrameworkSettings.TenantManagement.EmailSettings.UseHostDefault,
                input.UseHostDefaultEmailSettings.ToString());
            // todo: ? Clean up (maybe not)
        }

        public async Task ResetEmailSettingsAsync()
        {
            await SettingManager.SetForCurrentTenantAsync(EmailSettingNames.DefaultFromAddress, null);
            await SettingManager.SetForCurrentTenantAsync(EmailSettingNames.DefaultFromDisplayName, null);
            await SettingManager.SetForCurrentTenantAsync(EmailSettingNames.Smtp.Host, null);
            await SettingManager.SetForCurrentTenantAsync(EmailSettingNames.Smtp.Port, null);
            await SettingManager.SetForCurrentTenantAsync(EmailSettingNames.Smtp.UserName, null);
            await SettingManager.SetForCurrentTenantAsync(EmailSettingNames.Smtp.Password, null); // tod: Need decrypt?
            await SettingManager.SetForCurrentTenantAsync(EmailSettingNames.Smtp.Domain, null);
            await SettingManager.SetForCurrentTenantAsync(EmailSettingNames.Smtp.EnableSsl, null);
            await SettingManager.SetForCurrentTenantAsync(EmailSettingNames.Smtp.UseDefaultCredentials, null);

            await SettingManager
                .SetForCurrentTenantAsync(FrameworkSettings.TenantManagement.EmailSettings.UseHostDefault, null);
        }

        public async Task<PasswordComplexitySetting> GetPasswordComplexitySettingsAsync()
        {
            var passwordComplexitySetting = new PasswordComplexitySetting
            {
                RequireDigit = await SettingManager.GetForCurrentTenantAsync<bool>(IdentitySettingNames.Password.RequireDigit),
                RequireLowercase = await SettingManager.GetForCurrentTenantAsync<bool>(IdentitySettingNames.Password.RequireLowercase),
                RequireNonAlphanumeric = await SettingManager.GetForCurrentTenantAsync<bool>(IdentitySettingNames.Password.RequireNonAlphanumeric),
                RequireUppercase = await SettingManager.GetForCurrentTenantAsync<bool>(IdentitySettingNames.Password.RequireUppercase),
                RequiredLength = await SettingManager.GetForCurrentTenantAsync<int>(IdentitySettingNames.Password.RequiredLength)
            };

            return passwordComplexitySetting;
        }

        public async Task UpdatePasswordComplexitySettingsAsync(PasswordComplexitySetting settings)
        {
            await SettingManager.SetForCurrentTenantAsync(
                IdentitySettingNames.Password.RequireDigit,
                settings.RequireDigit.ToString()
            );
            await SettingManager.SetForCurrentTenantAsync(
                IdentitySettingNames.Password.RequireLowercase,
                settings.RequireLowercase.ToString()
            );
            await SettingManager.SetForCurrentTenantAsync(
                IdentitySettingNames.Password.RequireNonAlphanumeric,
                settings.RequireNonAlphanumeric.ToString()
            );
            await SettingManager.SetForCurrentTenantAsync(
                IdentitySettingNames.Password.RequireUppercase,
                settings.RequireUppercase.ToString()
            );
            await SettingManager.SetForCurrentTenantAsync(
                IdentitySettingNames.Password.RequiredLength,
                settings.RequiredLength.ToString()
            );
        }

        public async Task ResetPasswordComplexitySettingsAsync()
        {
            await SettingManager.SetForCurrentTenantAsync(
                IdentitySettingNames.Password.RequireDigit,
                null
            );
            await SettingManager.SetForCurrentTenantAsync(
                IdentitySettingNames.Password.RequireLowercase,
                null
            );
            await SettingManager.SetForCurrentTenantAsync(
                IdentitySettingNames.Password.RequireNonAlphanumeric,
                null
            );
            await SettingManager.SetForCurrentTenantAsync(
                IdentitySettingNames.Password.RequireUppercase,
                null
            );
            await SettingManager.SetForCurrentTenantAsync(
                IdentitySettingNames.Password.RequiredLength,
                null
            );
        }

        public async Task<UserLockOutSettingsEditDto> GetUserLockOutSettingsAsync()
        {
            return new UserLockOutSettingsEditDto
            {
                IsEnabled = await SettingManager.GetForCurrentTenantAsync<bool>(IdentitySettingNames.Lockout.AllowedForNewUsers),
                MaxFailedAccessAttemptsBeforeLockout = 
                await SettingManager.GetForCurrentTenantAsync<int>(IdentitySettingNames.Lockout.MaxFailedAccessAttempts),
                DefaultAccountLockoutSeconds = 
                await SettingManager.GetForCurrentTenantAsync<int>(IdentitySettingNames.Lockout.LockoutDuration)
            };
        }

        public async Task UpdateUserLockOutSettingsAsync(UserLockOutSettingsEditDto settings)
        {
            await SettingManager.SetForCurrentTenantAsync(
                IdentitySettingNames.Lockout.AllowedForNewUsers, 
                settings.IsEnabled.ToString().ToLowerInvariant());
            await SettingManager.SetForCurrentTenantAsync(
                IdentitySettingNames.Lockout.LockoutDuration, 
                settings.DefaultAccountLockoutSeconds.ToString());
            await SettingManager.SetForCurrentTenantAsync(
                IdentitySettingNames.Lockout.MaxFailedAccessAttempts, 
                settings.MaxFailedAccessAttemptsBeforeLockout.ToString());
        }

        public async Task ResetUserLockOutSettingsAsync()
        {
            await SettingManager.SetForCurrentTenantAsync(
                IdentitySettingNames.Lockout.AllowedForNewUsers,
                null
                );
            await SettingManager.SetForCurrentTenantAsync(
                IdentitySettingNames.Lockout.LockoutDuration,
                null);
            await SettingManager.SetForCurrentTenantAsync(
                IdentitySettingNames.Lockout.MaxFailedAccessAttempts,
                null);
        }

        public async Task<TwoFactorLoginSettingsEditDto> GetTwoFactorLoginSettingsAsync()
        {
            var settings = new TwoFactorLoginSettingsEditDto
            {
                IsEnabledForApplication = await SettingManager.GetForCurrentTenantAsync<bool>(FrameworkSettings.Features.IsTwoFactorOn)
            };

            // We are multi-tenancy system.
            if (!settings.IsEnabledForApplication)
            {
                return settings;
            }
            // On purpose, we use the setting provider.
            settings.IsEnabled = await SettingProvider.GetAsync<bool>(FrameworkSettings.Security.TwoFactor.IsEnabled);
            settings.IsRememberBrowserEnabled = await SettingProvider.GetAsync<bool>(FrameworkSettings.Security.TwoFactor.IsRememberBrowserEnabled);

            // Tenant specific
            settings.IsEmailProviderEnabled = await SettingProvider.GetAsync<bool>(FrameworkSettings.Security.TwoFactor.IsEmailProviderEnabled);
            settings.IsSmsProviderEnabled = await SettingProvider.GetAsync<bool>(FrameworkSettings.Security.TwoFactor.IsSmsProviderEnabled);
            settings.IsGoogleAuthenticatorEnabled = await SettingProvider.GetAsync<bool>(FrameworkSettings.Security.TwoFactor.IsGoogleAuthenticatorEnabled);

            return settings;
        }

        public async Task UpdateTwoFactorLoginSettingsAsync(TwoFactorLoginSettingsEditDto settings)
        {

            await SettingManager.SetForCurrentTenantAsync(
                FrameworkSettings.Security.TwoFactor.IsEnabled, 
                settings.IsEnabled.ToString().ToLowerInvariant());
            await SettingManager.SetForCurrentTenantAsync(
                FrameworkSettings.Security.TwoFactor.IsRememberBrowserEnabled, 
                settings.IsRememberBrowserEnabled.ToString().ToLowerInvariant());

            //These settings can only be changed by host, in a multitenant application.
            await SettingManager.SetForCurrentTenantAsync(
                FrameworkSettings.Security.TwoFactor.IsEmailProviderEnabled, 
                settings.IsEmailProviderEnabled.ToString().ToLowerInvariant());
            await SettingManager.SetForCurrentTenantAsync(
                FrameworkSettings.Security.TwoFactor.IsSmsProviderEnabled, 
                settings.IsSmsProviderEnabled.ToString().ToLowerInvariant());
            await SettingManager.SetForCurrentTenantAsync(
                FrameworkSettings.Security.TwoFactor.IsGoogleAuthenticatorEnabled, 
                settings.IsGoogleAuthenticatorEnabled.ToString().ToLowerInvariant());
        }

        public async Task ResetTwoFactorLoginSettingsAsync()
        {

            await SettingManager.SetForCurrentTenantAsync(
                FrameworkSettings.Security.TwoFactor.IsEnabled,
                null);
            await SettingManager.SetForCurrentTenantAsync(
                FrameworkSettings.Security.TwoFactor.IsRememberBrowserEnabled,
                null);

            //These settings can only be changed by host, in a multitenant application.
            await SettingManager.SetForCurrentTenantAsync(
                FrameworkSettings.Security.TwoFactor.IsEmailProviderEnabled,
                null);
            await SettingManager.SetForCurrentTenantAsync(
                FrameworkSettings.Security.TwoFactor.IsSmsProviderEnabled,
                null
                );
            await SettingManager.SetForCurrentTenantAsync(
                FrameworkSettings.Security.TwoFactor.IsGoogleAuthenticatorEnabled,
                null
                );
        }


        public async Task<TenantUserManagementSettingsEditDto> GetUserManagementSettingsAsync()
        {
            return new TenantUserManagementSettingsEditDto
            {
                AllowSelfRegistration = await SettingManager.GetForCurrentTenantAsync<bool>(AccountSettingNames.IsSelfRegistrationEnabled),
                // Obsoelete
                IsNewRegisteredUserActiveByDefault = false,
                // Obsolete
                IsEmailConfirmationRequiredForLogin = false,
                RegistrationApprovalType = await SettingManager.GetForCurrentTenantAsync<int>(FrameworkSettings.Account.RegistrationApprovalType),
                UseCaptchaOnRegistration = await SettingManager.GetForCurrentTenantAsync<bool>(FrameworkSettings.Security.UseCaptchaOnRegistration),
                UseCaptchaOnLogin = await SettingManager.GetForCurrentTenantAsync<bool>(FrameworkSettings.Security.UseCaptchaOnLogin),
                IsCookieConsentEnabled = await SettingManager.GetForCurrentTenantAsync<bool>(FrameworkSettings.DataPrivacy.IsCookieConsentEnabled),
                IsQuickThemeSelectEnabled = false, // todo:
                AllowUsingGravatarProfilePicture = true, // todo:
                SessionTimeOutSettings = new SessionTimeOutSettingsEditDto()
                {
                    IsEnabled = await SettingManager.GetForCurrentTenantAsync<bool>(FrameworkSettings.Security.SessionTimeOut.IsEnabled),
                    TimeOutSecond = await SettingManager.GetForCurrentTenantAsync<int>(FrameworkSettings.Security.SessionTimeOut.TimeOutSecond),
                    ShowTimeOutNotificationSecond = await SettingManager.GetForCurrentTenantAsync<int>(FrameworkSettings.Security.SessionTimeOut.ShowTimeOutNotificationSecond),
                    ShowLockScreenWhenTimedOut = await SettingManager.GetForCurrentTenantAsync<bool>(FrameworkSettings.Security.SessionTimeOut.ShowLockScreenWhenTimedOut)
                }
            };
        }

        public async Task UpdateUserManagementSettingsAsync(TenantUserManagementSettingsEditDto settings)
        {
            await SettingManager.SetForCurrentTenantAsync(
                AccountSettingNames.IsSelfRegistrationEnabled,
                settings.AllowSelfRegistration.ToString().ToLowerInvariant()
            );

            // todo: Registration type

            await SettingManager.SetForCurrentTenantAsync(
                FrameworkSettings.Security.UseCaptchaOnRegistration,
                settings.UseCaptchaOnRegistration.ToString().ToLowerInvariant()
            );
            await SettingManager.SetForCurrentTenantAsync(
                FrameworkSettings.Security.UseCaptchaOnLogin,
                settings.UseCaptchaOnLogin.ToString().ToLowerInvariant()
            );
            await SettingManager.SetForCurrentTenantAsync(
                FrameworkSettings.DataPrivacy.IsCookieConsentEnabled,
                settings.IsCookieConsentEnabled.ToString().ToLowerInvariant()
            );

            // todo: Avatar

            await UpdateUserManagementSessionTimeOutSettingsAsync(settings.SessionTimeOutSettings);
        }

        public async Task UpdateUserManagementSessionTimeOutSettingsAsync(SessionTimeOutSettingsEditDto settings)
        {
            await SettingManager.SetForCurrentTenantAsync(
                FrameworkSettings.Security.SessionTimeOut.IsEnabled,
                settings.IsEnabled.ToString().ToLowerInvariant()
            );
            await SettingManager.SetForCurrentTenantAsync(
                FrameworkSettings.Security.SessionTimeOut.TimeOutSecond,
                settings.TimeOutSecond.ToString()
            );
            await SettingManager.SetForCurrentTenantAsync(
                FrameworkSettings.Security.SessionTimeOut.ShowTimeOutNotificationSecond,
                settings.ShowTimeOutNotificationSecond.ToString()
            );
            await SettingManager.SetForCurrentTenantAsync(
                FrameworkSettings.Security.SessionTimeOut.ShowLockScreenWhenTimedOut,
                settings.ShowLockScreenWhenTimedOut.ToString()
            );
        }

        public async Task ResetUserManagementSettingsAsync()
        {
            await SettingManager.SetForCurrentTenantAsync(
                AccountSettingNames.IsSelfRegistrationEnabled, null
            );

            // todo: Registration type

            await SettingManager.SetForCurrentTenantAsync(
                FrameworkSettings.Security.UseCaptchaOnRegistration, null
            );
            await SettingManager.SetForCurrentTenantAsync(
                FrameworkSettings.Security.UseCaptchaOnLogin, null
            );
            await SettingManager.SetForCurrentTenantAsync(
                FrameworkSettings.DataPrivacy.IsCookieConsentEnabled,
                null
            );

            // todo: Avatar

            await ResetUserManagementSessionTimeOutSettingsAsync();
        }

        public async Task ResetUserManagementSessionTimeOutSettingsAsync()
        {
            await SettingManager.SetForCurrentTenantAsync(
                FrameworkSettings.Security.SessionTimeOut.IsEnabled,
                null
            );
            await SettingManager.SetForCurrentTenantAsync(
                FrameworkSettings.Security.SessionTimeOut.TimeOutSecond,
                null
            );
            await SettingManager.SetForCurrentTenantAsync(
                FrameworkSettings.Security.SessionTimeOut.ShowTimeOutNotificationSecond,
                null
            );
            await SettingManager.SetForCurrentTenantAsync(
                FrameworkSettings.Security.SessionTimeOut.ShowLockScreenWhenTimedOut,
                null
            );
        }
    }
}
