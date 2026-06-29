using MediatR;
using Microsoft.EntityFrameworkCore;
using ProfileService.Data;
using ProfileService.Domain.Entities;
using ProfileService.Infrastructure;
using Repository.Layer.Interfaces;

namespace ProfileService.Features.Settings.UpdateSettings;

public sealed record UpdateSettingsCommand(
    UserPreferencesPatch? Preferences,
    NotificationSettingsPatch? Notifications,
    PrivacySettingsPatch? Privacy) : IRequest<OperationResult<SettingsResponse>>;

public sealed class UpdateSettingsHandler(
    IClaimsManager claims,
    IUnitOfWork<ProfileDbContext> unitOfWork) : IRequestHandler<UpdateSettingsCommand, OperationResult<SettingsResponse>>
{
    public async Task<OperationResult<SettingsResponse>> Handle(UpdateSettingsCommand request, CancellationToken cancellationToken)
    {
        var profile = await unitOfWork.Repository<UserProfile, Guid>()
            .Query(
                asNoTracking: false,
                x => x.Preferences!,
                x => x.NotificationSettings!,
                x => x.PrivacySettings!)
            .SingleOrDefaultAsync(x => x.UserId == claims.UserId, cancellationToken);

        if (profile is null)
            return OperationResultFactory.NotFound<SettingsResponse>("Profile was not found.", "لم يتم العثور على الملف الشخصي");

        profile.Preferences ??= new UserPreferences { UserId = profile.UserId };
        profile.NotificationSettings ??= new NotificationSettings { UserId = profile.UserId };
        profile.PrivacySettings ??= new PrivacySettings { UserId = profile.UserId };

        var validationError = ApplyPreferences(profile.Preferences, request.Preferences)
            ?? ApplyNotifications(profile.NotificationSettings, request.Notifications)
            ?? ApplyPrivacy(profile.PrivacySettings, request.Privacy);
        if (validationError is not null)
            return OperationResultFactory.BadRequest<SettingsResponse>(validationError, validationError);

        await unitOfWork.CompleteAsync();

        return OperationResultFactory.Success(new SettingsResponse(
            new UserPreferencesDto(
                profile.Preferences.Language,
                profile.Preferences.Theme,
                profile.Preferences.WeightUnit,
                profile.Preferences.HeightUnit,
                profile.Preferences.DistanceUnit),
            new NotificationSettingsDto(
                profile.NotificationSettings.WorkoutReminders,
                profile.NotificationSettings.MealReminders,
                profile.NotificationSettings.AchievementAlerts,
                profile.NotificationSettings.EmailNotifications,
                profile.NotificationSettings.WeeklyReports,
                profile.NotificationSettings.PushNotifications),
            new PrivacySettingsDto(
                profile.PrivacySettings.ProfileVisibility,
                profile.PrivacySettings.ShowProgressToFriends,
                profile.PrivacySettings.AllowDataSharing)));
    }

    private static string? ApplyPreferences(UserPreferences target, UserPreferencesPatch? patch)
    {
        if (patch is null)
            return null;

        target.Language = Normalize(patch.Language, target.Language);
        target.Theme = Normalize(patch.Theme, target.Theme);
        target.WeightUnit = Normalize(patch.WeightUnit, target.WeightUnit);
        target.HeightUnit = Normalize(patch.HeightUnit, target.HeightUnit);
        target.DistanceUnit = Normalize(patch.DistanceUnit, target.DistanceUnit);

        return target.Language.Length > 10 || target.Theme.Length > 15 ||
               target.WeightUnit.Length > 5 || target.HeightUnit.Length > 5 || target.DistanceUnit.Length > 5
            ? "One or more preference values exceed the allowed length."
            : null;
    }

    private static string? ApplyNotifications(NotificationSettings target, NotificationSettingsPatch? patch)
    {
        if (patch is null)
            return null;

        target.WorkoutReminders = patch.WorkoutReminders ?? target.WorkoutReminders;
        target.MealReminders = patch.MealReminders ?? target.MealReminders;
        target.AchievementAlerts = patch.AchievementAlerts ?? target.AchievementAlerts;
        target.EmailNotifications = patch.EmailNotifications ?? target.EmailNotifications;
        target.WeeklyReports = patch.WeeklyReports ?? target.WeeklyReports;
        target.PushNotifications = patch.PushNotifications ?? target.PushNotifications;
        return null;
    }

    private static string? ApplyPrivacy(PrivacySettings target, PrivacySettingsPatch? patch)
    {
        if (patch is null)
            return null;

        target.ProfileVisibility = Normalize(patch.ProfileVisibility, target.ProfileVisibility);
        target.ShowProgressToFriends = patch.ShowProgressToFriends ?? target.ShowProgressToFriends;
        target.AllowDataSharing = patch.AllowDataSharing ?? target.AllowDataSharing;

        return target.ProfileVisibility.Length > 20
            ? "Profile visibility exceeds the allowed length."
            : null;
    }

    private static string Normalize(string? value, string current) =>
        string.IsNullOrWhiteSpace(value) ? current : value.Trim();
}
