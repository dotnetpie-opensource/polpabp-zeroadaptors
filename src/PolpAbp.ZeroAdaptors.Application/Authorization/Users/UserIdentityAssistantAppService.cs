using Microsoft.AspNetCore.Identity;
using PolpAbp.ZeroAdaptors.Security;
using System;
using System.Collections.Generic;
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
                if (string.IsNullOrEmpty(password))
                {
                    return IdentityResult.Failed(new IdentityError
                    {
                        Code = PasswordValidationErrors.PasswordRequired
                    });
                }

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

        public Task<string> CreateRandomPasswordAsync()
        {
            // todo: Read from settings
            var passwordComplexitySetting =  PasswordComplexitySetting.DefaultSettings;

            var upperCaseLetters = "ABCDEFGHJKLMNOPQRSTUVWXYZ";
            var lowerCaseLetters = "abcdefghijkmnopqrstuvwxyz";
            var digits = "0123456789";
            var nonAlphanumerics = "!@$?_-";

            string[] randomChars = {
                upperCaseLetters,
                lowerCaseLetters,
                digits,
                nonAlphanumerics
            };

            var rand = new Random(Environment.TickCount);
            var chars = new List<char>();

            if (passwordComplexitySetting.RequireUppercase)
            {
                chars.Insert(rand.Next(0, chars.Count),
                    upperCaseLetters[rand.Next(0, upperCaseLetters.Length)]);
            }

            if (passwordComplexitySetting.RequireLowercase)
            {
                chars.Insert(rand.Next(0, chars.Count),
                    lowerCaseLetters[rand.Next(0, lowerCaseLetters.Length)]);
            }

            if (passwordComplexitySetting.RequireDigit)
            {
                chars.Insert(rand.Next(0, chars.Count),
                    digits[rand.Next(0, digits.Length)]);
            }

            if (passwordComplexitySetting.RequireNonAlphanumeric)
            {
                chars.Insert(rand.Next(0, chars.Count),
                    nonAlphanumerics[rand.Next(0, nonAlphanumerics.Length)]);
            }

            for (var i = chars.Count; i < passwordComplexitySetting.RequiredLength; i++)
            {
                var rcs = randomChars[rand.Next(0, randomChars.Length)];
                chars.Insert(rand.Next(0, chars.Count),
                    rcs[rand.Next(0, rcs.Length)]);
            }

            var ret = new string(chars.ToArray());

            return Task.FromResult(ret);
        }

    }
}
