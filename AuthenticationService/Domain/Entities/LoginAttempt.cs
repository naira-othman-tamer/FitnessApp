namespace AuthenticationService.Domain.Entities;

public sealed class LoginAttempt
{
    public Guid Id { get; set; }
    public required string Email { get; set; }
    public DateTime AttemptedAt { get; set; }
    public bool IsSuccess { get; set; }
    public required string IpAddress { get; set; }
}
