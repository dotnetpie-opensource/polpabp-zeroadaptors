using PolpAbp.Framework;
using System.Threading.Tasks;
using Volo.Abp.Data;
using Volo.Abp.Identity;
using Xunit;

namespace PolpAbp.ZeroAdaptors.Authorization.Users.Profile
{
    public class ProfileAppServiceTests : ZeroAdaptorsApplicationTestBase4Admin
    {
        private readonly IProfileAppService _profileAppService;
        private readonly IdentityUserManager _identityUserManager;

        public ProfileAppServiceTests() : base()
        {
            _profileAppService = GetRequiredService<IProfileAppService>();
            _identityUserManager = GetRequiredService<IdentityUserManager>();
        }

        [Fact]
        public async Task CanChangePasswordAsync()
        {
            var newPass = "1234Asdfzxc!@";
            await _profileAppService.ChangePasswordAsync(new Dto.ChangePasswordInput
            {
                CurrentPassword = FrameworkTestConsts.AdminPass,
                NewPassword = newPass
            });

            var adminUser = await _identityUserManager.GetByIdAsync(FrameworkTestConsts.AdminId);

            // Next verify its password 
            var ret = await _identityUserManager.CheckPasswordAsync(adminUser, newPass);

            Assert.True(ret);
        }

        [Fact]
        public async Task CanGetUserProfileAsync()
        {
            var info = await _profileAppService.GetCurrentUserProfileForEditAsync();

            Assert.Equal(info.EmailAddress.ToUpper(), FrameworkTestConsts.AdminEmail.ToUpper());
        }

        [Fact]
        public async Task CanUpdateUserProfileAsync()
        {
            var info = await _profileAppService.GetCurrentUserProfileForEditAsync();
            info.UserName = "another@gmail.com";
            info.EmailAddress = "another@gmail.com";
            info.PhoneNumber = "5555555555";
            info.Surname = "Hello";
            info.Name = "World";
            await _profileAppService.UpdateCurrentUserProfileAsync(info);
            var newInfo = await _profileAppService.GetCurrentUserProfileForEditAsync();

            Assert.Equal(info.EmailAddress.ToUpper(), newInfo.EmailAddress.ToUpper());
            Assert.Equal(info.UserName.ToUpper(), newInfo.UserName.ToUpper());
            Assert.Equal(info.PhoneNumber.ToUpper(), newInfo.PhoneNumber.ToUpper());
            Assert.Equal(info.Name.ToUpper(), newInfo.Name.ToUpper());
            Assert.Equal(info.Surname.ToUpper(), newInfo.Surname.ToUpper());
        }

    }
}
