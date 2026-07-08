using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

namespace NutritionService.Infrastructure;

public interface IClaimsManager
{
    Guid UserId { get; }
    bool IsInRole(string role);
}

public sealed class ClaimsManager(IHttpContextAccessor accessor) : IClaimsManager
{
    private ClaimsPrincipal? Principal => accessor.HttpContext?.User;

    public Guid UserId => Guid.TryParse(
        Principal?.FindFirstValue(ClaimTypes.NameIdentifier) ?? Principal?.FindFirstValue(JwtRegisteredClaimNames.Sub),
        out var userId)
        ? userId
        : throw new UnauthorizedAccessException("The authenticated user identifier claim is missing or invalid.");

    public bool IsInRole(string role) => Principal?.IsInRole(role) == true;
}
