using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using AuthenticationService.Domain.Entities;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;

namespace AuthenticationService.Infrastructure;

public sealed record IssuedAccessToken(string Token, string Jti, DateTime ExpiresAtUtc);
public sealed record ResetTokenClaims(Guid UserId, string SecurityStamp);

public interface ITokenService
{
    IssuedAccessToken CreateAccessToken(ApplicationUser user, IEnumerable<string>? roles = null);
    string CreateRefreshToken();
    string CreateResetToken(ApplicationUser user);
    ResetTokenClaims ValidateResetToken(string token);
    string HashOpaqueToken(string token);
}

public sealed class TokenService(IOptions<JwtOptions> options) : ITokenService
{
    private readonly JwtOptions _options = options.Value;

    public IssuedAccessToken CreateAccessToken(ApplicationUser user, IEnumerable<string>? roles = null)
    {
        var jti = Guid.NewGuid().ToString("N");
        var expires = DateTime.UtcNow.AddMinutes(_options.AccessTokenMinutes);
        var claims = new List<Claim>
        {
            new Claim(JwtRegisteredClaimNames.Sub, user.Id.ToString()),
            new Claim(JwtRegisteredClaimNames.Email, user.Email!),
            new Claim(JwtRegisteredClaimNames.Jti, jti),
            new Claim(ClaimTypes.NameIdentifier, user.Id.ToString())
        };
        claims.AddRange((roles ?? []).Select(role => new Claim(ClaimTypes.Role, role)));
        return new IssuedAccessToken(WriteToken(claims, expires), jti, expires);
    }

    public string CreateRefreshToken() => Convert.ToBase64String(RandomNumberGenerator.GetBytes(64));

    public string CreateResetToken(ApplicationUser user)
    {
        var claims = new[]
        {
            new Claim(JwtRegisteredClaimNames.Sub, user.Id.ToString()),
            new Claim("purpose", "password-reset"),
            new Claim("security_stamp", user.SecurityStamp ?? string.Empty),
            new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString("N"))
        };
        return WriteToken(claims, DateTime.UtcNow.AddMinutes(_options.ResetTokenMinutes));
    }

    public ResetTokenClaims ValidateResetToken(string token)
    {
        var principal = new JwtSecurityTokenHandler().ValidateToken(
            token,
            CreateValidationParameters(_options),
            out _);
        if (principal.FindFirst("purpose")?.Value != "password-reset" ||
            !Guid.TryParse(principal.FindFirst(JwtRegisteredClaimNames.Sub)?.Value, out var userId))
            throw new SecurityTokenException("Invalid reset token.");
        return new ResetTokenClaims(userId, principal.FindFirst("security_stamp")?.Value ?? string.Empty);
    }

    public string HashOpaqueToken(string token) =>
        Convert.ToBase64String(SHA256.HashData(Encoding.UTF8.GetBytes(token)));

    public static TokenValidationParameters CreateValidationParameters(JwtOptions options) => new()
    {
        ValidateIssuerSigningKey = true,
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(options.Key)),
        ValidateIssuer = true,
        ValidIssuer = options.Issuer,
        ValidateAudience = true,
        ValidAudience = options.Audience,
        ValidateLifetime = true,
        ClockSkew = TimeSpan.Zero
    };

    private string WriteToken(IEnumerable<Claim> claims, DateTime expires)
    {
        var token = new JwtSecurityToken(
            _options.Issuer,
            _options.Audience,
            claims,
            expires: expires,
            signingCredentials: new SigningCredentials(
                new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_options.Key)),
                SecurityAlgorithms.HmacSha256));
        return new JwtSecurityTokenHandler().WriteToken(token);
    }
}
