namespace SportsGurukul.Application.Features.Authentication.Constants;

public static class AuthenticationConstants
{
    public const int MaxFailedLoginAttempts = 5;
    public const int LockoutDurationMinutes = 15;
    public const int PasswordMinLength = 8;
    public const string DefaultRole = "Athlete";
    public const int AccessTokenExpirationMinutes = 60;
    public const int RefreshTokenExpirationDays = 30;
}
