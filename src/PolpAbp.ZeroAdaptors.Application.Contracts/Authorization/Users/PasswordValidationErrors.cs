namespace PolpAbp.ZeroAdaptors.Authorization.Users
{
    public static class PasswordValidationErrors
    {
        public const string PasswordRequired = nameof(PasswordRequired);
        public const string InvalidMinLength = nameof(InvalidMinLength);
        public const string UppercaseRequired = nameof(UppercaseRequired);
        public const string LowercaseRequired = nameof(LowercaseRequired);
        public const string DigitRequired = nameof(DigitRequired);
        public const string NonAlphaRequired = nameof(NonAlphaRequired);
    }
}
