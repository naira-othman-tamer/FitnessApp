namespace AuthenticationService.Infrastructure;

public sealed class JwtOptions
{
    public const string SectionName = "Jwt";
    public required string Key { get; init; }
    public required string Issuer { get; init; }
    public required string Audience { get; init; }
    public int AccessTokenMinutes { get; init; } = 15;
    public int RefreshTokenDays { get; init; } = 7;
    public int ResetTokenMinutes { get; init; } = 10;

    public void Validate()
    {
        if (Key.Length < 32)
            throw new InvalidOperationException("Jwt:Key must contain at least 32 characters.");
        if (AccessTokenMinutes <= 0 || RefreshTokenDays <= 0 || ResetTokenMinutes <= 0)
            throw new InvalidOperationException("JWT lifetimes must be positive.");
    }
}
