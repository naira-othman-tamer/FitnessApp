namespace ProfileService.Domain.Entities;

public sealed class PrivacySettings
{
    public Guid UserId { get; set; }
    public string ProfileVisibility { get; set; } = "private";
    public bool ShowProgressToFriends { get; set; }
    public bool AllowDataSharing { get; set; }

    public UserProfile? UserProfile { get; set; }
}
