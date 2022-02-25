namespace PolpAbp.ZeroAdaptors.Authorization.Users.Dto
{
    public class ResetUserPasswordDto
    {
        public bool SetRandomPassword { get; set; }
        public string Password { get; set; }
        public bool ShouldChangePasswordOnNextLogin { get; set; }
    }
}
