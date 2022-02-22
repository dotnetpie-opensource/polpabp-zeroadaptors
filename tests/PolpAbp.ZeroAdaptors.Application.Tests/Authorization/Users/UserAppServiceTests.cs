using PolpAbp.Framework;
using PolpAbp.ZeroAdaptors.Authorization.Users.Dto;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Volo.Abp.Identity;
using Xunit;

namespace PolpAbp.ZeroAdaptors.Authorization.Users
{
    public class UserAppServiceTests : ZeroAdaptorsApplicationTestBase4Admin
    {
        private readonly IUserAppService _userAppService;
        private readonly IdentityUserManager _identityUserManager;

        public UserAppServiceTests()
        {
            _userAppService = GetRequiredService<IUserAppService>();
            _identityUserManager = GetRequiredService<IdentityUserManager>();
        }

        [Fact]
        public async Task CanGetUserForCreateAsync()
        {
            var ret = await _userAppService.GetUserForCreateAsync();
            Assert.NotNull(ret);
            Assert.True(ret.User.ShouldChangePasswordOnNextLogin);
        }

        [Fact]
        public async Task CanGetUserForEditAsync()
        {
            var ret = await _userAppService.GetUserForEditAsync(FrameworkTestConsts.MemberUserId1);
            Assert.NotNull(ret);
        }

        [Fact]
        public async Task CanCreateUserWithRandomPasswordAsync()
        {
            var ret = await _userAppService.GetUserForCreateAsync();
            var inputModel = new CreateOrUpdateUserInput
            {
                SetRandomPassword = true,
                User = new UserEditDto
                {
                    Name = "Richard",
                    Surname = "Swift",
                    UserName = "RichardSwift",
                    EmailAddress = "Richard@gmail.com",
                    IsLockoutEnabled = true
                }
            };

            var id = await _userAppService.CreateUserAsync(inputModel);
            Assert.True(true);
        }

        [Fact]
        public async Task CanCreateUserWithGivenPasswordAsync()
        {
            var ret = await _userAppService.GetUserForCreateAsync();
            var inputModel = new CreateOrUpdateUserInput
            {
                User = new UserEditDto
                {
                    Name = "Richard",
                    Surname = "Swift",
                    UserName = "RichardSwift",
                    EmailAddress = "Richard@gmail.com",
                    IsLockoutEnabled = true,
                    Password = "1q2w3eA!"
                }
            };

            var id = await _userAppService.CreateUserAsync(inputModel);
            Assert.True(true);

        }

        [Fact]
        public async Task CanUpdateUserAsync()
        {
            var ret = await _userAppService.GetUserForEditAsync(FrameworkTestConsts.MemberUserId1);
            var newModel = new CreateOrUpdateUserInput
            {
                User = new UserEditDto
                {
                    Id = ret.User.Id,
                    UserName = ret.User.UserName,
                    Name = "Jack",
                    Surname = "T",
                    EmailAddress = ret.User.EmailAddress,
                    IsLockoutEnabled = ret.User.LockoutEnabled,
                    IsTwoFactorEnabled = ret.User.IsTwoFactorEnabled,
                    PhoneNumber = ret.User.PhoneNumber,
                    ShouldChangePasswordOnNextLogin = true,
                }
            };

            await _userAppService.UpdateUserAsync(newModel);
            Assert.True(true);
        }

        [Fact]
        public async Task CanUpdateUserWithRandomPasswordAsync()
        {
            var ret = await _userAppService.GetUserForEditAsync(FrameworkTestConsts.MemberUserId1);
            var newModel = new CreateOrUpdateUserInput
            {
                SetRandomPassword = true,
                User = new UserEditDto
                {
                    Id = ret.User.Id,
                    UserName = ret.User.UserName,
                    Name = "Jack",
                    Surname = "T",
                    EmailAddress = ret.User.EmailAddress,
                    IsLockoutEnabled = ret.User.LockoutEnabled,
                    IsTwoFactorEnabled = ret.User.IsTwoFactorEnabled,
                    PhoneNumber = ret.User.PhoneNumber,
                    ShouldChangePasswordOnNextLogin = true,
                }
            };

            await _userAppService.UpdateUserAsync(newModel);
            Assert.True(true);
        }

        [Fact]
        public async Task CanUpdateUserWithGivenPasswordAsync()
        {
            var ret = await _userAppService.GetUserForEditAsync(FrameworkTestConsts.MemberUserId1);
            var newModel = new CreateOrUpdateUserInput
            {
                User = new UserEditDto
                {
                    Id = ret.User.Id,
                    UserName = ret.User.UserName,
                    Name = "Jack",
                    Surname = "T",
                    EmailAddress = ret.User.EmailAddress,
                    IsLockoutEnabled = ret.User.LockoutEnabled,
                    IsTwoFactorEnabled = ret.User.IsTwoFactorEnabled,
                    PhoneNumber = ret.User.PhoneNumber,
                    ShouldChangePasswordOnNextLogin = true,
                    Password = "1q2w3eA!"
                }
            };

            await _userAppService.UpdateUserAsync(newModel);
            Assert.True(true);

            var newUser = await _identityUserManager.GetByIdAsync(FrameworkTestConsts.MemberUserId1);
            var flag = await _identityUserManager.CheckPasswordAsync(newUser, newModel.User.Password);
            Assert.True(flag);
        }

        [Fact]
        public async Task CanResetUserPasswordWithGivenPasswordAsync()
        {
            var inputDto = new ResetUserPasswordDto
            {
                Password = "1a2w3eA!"
            };
            await _userAppService.ResetUserPasswordAsync(FrameworkTestConsts.MemberUserId1, inputDto, true);

            var newUser = await _identityUserManager.GetByIdAsync(FrameworkTestConsts.MemberUserId1);
            var flag = await _identityUserManager.CheckPasswordAsync(newUser, inputDto.Password);
            Assert.True(flag);

        }

        [Fact]
        public async Task CanResetUserPasswordWithRandomPasswordAsync()
        {
            var inputDto = new ResetUserPasswordDto
            {
                SetRandomPassword = true
            };
            var ex = await Record.ExceptionAsync(async () =>
            {
                await _userAppService.ResetUserPasswordAsync(FrameworkTestConsts.MemberUserId1, inputDto, true);
            });
            Assert.Null(ex);
        }
    }
}
