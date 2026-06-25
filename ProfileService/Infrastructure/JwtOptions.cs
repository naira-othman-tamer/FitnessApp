namespace ProfileService.Infrastructure;

public sealed class JwtOptions
{
    public const string SectionName = "Jwt";
    public required string Key { get; init; }
    public required string Issuer { get; init; }
    public required string Audience { get; init; }

    public void Validate()
    {
        if (Key.Length < 32)
            throw new InvalidOperationException("Jwt:Key must contain at least 32 characters.");
    }
}
