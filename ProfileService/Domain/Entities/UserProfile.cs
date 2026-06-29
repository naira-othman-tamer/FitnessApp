namespace ProfileService.Domain.Entities;

public sealed class UserProfile
{
    public Guid UserId { get; set; }
    public required string FirstName { get; set; }
    public required string LastName { get; set; }
    public required string Email { get; set; }
    public required string PhoneNumber { get; set; }
    public string? ProfilePictureUrl { get; set; }
    public bool IsPremiumCached { get; set; }
    public DateTime MemberSince { get; set; }

    public UserPreferences? Preferences { get; set; }
    public NotificationSettings? NotificationSettings { get; set; }
    public PrivacySettings? PrivacySettings { get; set; }
}
