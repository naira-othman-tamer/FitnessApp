namespace ProfileService.Features.Settings;

public sealed record SettingsResponse(
    UserPreferencesDto Preferences,
    NotificationSettingsDto Notifications,
    PrivacySettingsDto Privacy);

public sealed record UserPreferencesDto(
    string Language,
    string Theme,
    string WeightUnit,
    string HeightUnit,
    string DistanceUnit);

public sealed record NotificationSettingsDto(
    bool WorkoutReminders,
    bool MealReminders,
    bool AchievementAlerts,
    bool EmailNotifications,
    bool WeeklyReports,
    bool PushNotifications);

public sealed record PrivacySettingsDto(
    string ProfileVisibility,
    bool ShowProgressToFriends,
    bool AllowDataSharing);

public sealed record UserPreferencesPatch(
    string? Language,
    string? Theme,
    string? WeightUnit,
    string? HeightUnit,
    string? DistanceUnit);

public sealed record NotificationSettingsPatch(
    bool? WorkoutReminders,
    bool? MealReminders,
    bool? AchievementAlerts,
    bool? EmailNotifications,
    bool? WeeklyReports,
    bool? PushNotifications);

public sealed record PrivacySettingsPatch(
    string? ProfileVisibility,
    bool? ShowProgressToFriends,
    bool? AllowDataSharing);
