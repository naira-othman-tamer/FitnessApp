namespace AuthenticationService.Domain.Entities;

public sealed class OtpCode
{
    public Guid Id { get; set; }
    public required string Email { get; set; }
    public required string Code { get; set; }
    public DateTime ExpiresAt { get; set; }
    public bool IsUsed { get; set; }
    public DateTime CreatedAt { get; set; }
}
