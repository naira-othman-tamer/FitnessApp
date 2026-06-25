using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

namespace ProfileService.Infrastructure;

public interface IClaimsManager
{
    bool IsAuthenticated { get; }
    Guid UserId { get; }
    string? Email { get; }
    bool TryGetUserId(out Guid userId);
    string? GetClaim(string claimType);
}

public sealed class ClaimsManager(IHttpContextAccessor httpContextAccessor) : IClaimsManager
{
    private ClaimsPrincipal? Principal => httpContextAccessor.HttpContext?.User;

    public bool IsAuthenticated => Principal?.Identity?.IsAuthenticated == true;

    public Guid UserId => TryGetUserId(out var userId)
        ? userId
        : throw new UnauthorizedAccessException("The authenticated user identifier claim is missing or invalid.");

    public string? Email => GetClaim(ClaimTypes.Email) ?? GetClaim(JwtRegisteredClaimNames.Email);

    public bool TryGetUserId(out Guid userId) =>
        Guid.TryParse(GetClaim(ClaimTypes.NameIdentifier) ?? GetClaim(JwtRegisteredClaimNames.Sub), out userId);

    public string? GetClaim(string claimType) => Principal?.FindFirst(claimType)?.Value;
}
