using Microsoft.AspNetCore.Identity;
using PolpAbp.ZeroAdaptors.Security;
using System.Linq;
using System.Threading.Tasks;
using Volo.Abp;

namespace PolpAbp.ZeroAdaptors.Authorization.Users
{
    [RemoteService(false)]
    public class UserIdentityAssistantAppService : ZeroAdaptorsAppService, IUserIdentityAssistantAppService
    {
        public Task<IdentityResult> ValidatePasswordAsync(string password)
        {
            return Task.Run(() =>
            {
                // todo: Load the tenant-specific settings form db.
                var settings = PasswordComplexitySetting.DefaultSettings;

                if (password.Length < settings.RequiredLength)
                {
                    return IdentityResult.Failed(new IdentityError
                    {
                        Code = PasswordValidationErrors.InvalidMinLength
                    });
                }

                if (settings.RequireUppercase && !password.Any(char.IsUpper))
                {
                    return IdentityResult.Failed(new IdentityError
                    {
                        Code = PasswordValidationErrors.UppercaseRequired
                    });
                }

                if (settings.RequireLowercase && !password.Any(char.IsLower))
                {
                    return IdentityResult.Failed(new IdentityError
                    {
                        Code = PasswordValidationErrors.LowercaseRequired
                    });
                }

                if (settings.RequireDigit && !password.Any(char.IsNumber))
                {
                    return IdentityResult.Failed(new IdentityError
                    {
                        Code = PasswordValidationErrors.DigitRequired
                    });
                }

                if (settings.RequireNonAlphanumeric && password.All(char.IsLetterOrDigit))
                {
                    return IdentityResult.Failed(new IdentityError
                    {
                        Code = PasswordValidationErrors.NonAlphaRequired
                    });
                }

                return IdentityResult.Success;

            });
        }
    }
}
