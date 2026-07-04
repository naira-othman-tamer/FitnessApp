namespace NutritionService.Infrastructure;

public sealed class JwtOptions
{
    public const string SectionName = "Jwt";
    public required string Key { get; init; }
    public required string Issuer { get; init; }
    public required string Audience { get; init; }

    public void Validate()
    {
        if (Key.Length < 32) throw new InvalidOperationException("JWT key must be at least 32 characters.");
        if (string.IsNullOrWhiteSpace(Issuer) || string.IsNullOrWhiteSpace(Audience))
            throw new InvalidOperationException("JWT issuer and audience are required.");
    }
}
