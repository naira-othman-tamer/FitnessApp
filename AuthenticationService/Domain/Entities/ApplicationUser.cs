using Microsoft.AspNetCore.Identity;

namespace AuthenticationService.Domain.Entities;

public sealed class ApplicationUser : IdentityUser<Guid>
{
    public bool IsLockedOut { get; set; }
    public DateTime? LockedUntil { get; set; }
    public DateTime CreatedAt { get; set; }
    public ICollection<RefreshToken> RefreshTokens { get; set; } = [];
}
