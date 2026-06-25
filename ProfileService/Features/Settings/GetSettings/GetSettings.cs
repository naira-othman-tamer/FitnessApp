using MediatR;
using Microsoft.EntityFrameworkCore;
using ProfileService.Data;
using ProfileService.Domain.Entities;
using ProfileService.Infrastructure;
using Repository.Layer.Interfaces;

namespace ProfileService.Features.Settings.GetSettings;

public sealed record GetSettingsQuery : IRequest<OperationResult<SettingsResponse>>;

public sealed class GetSettingsHandler(
    IClaimsManager claims,
    IUnitOfWork<ProfileDbContext> unitOfWork) : IRequestHandler<GetSettingsQuery, OperationResult<SettingsResponse>>
{
    public async Task<OperationResult<SettingsResponse>> Handle(GetSettingsQuery request, CancellationToken cancellationToken)
    {
        var profile = await unitOfWork.Repository<UserProfile, Guid>()
            .Query(
                true,
                x => x.Preferences!,
                x => x.NotificationSettings!,
                x => x.PrivacySettings!)
            .SingleOrDefaultAsync(x => x.UserId == claims.UserId, cancellationToken);

        if (profile is null)
            return OperationResultFactory.NotFound<SettingsResponse>("Profile was not found.", "لم يتم العثور على الملف الشخصي");

        var preferences = profile.Preferences ?? new UserPreferences { UserId = profile.UserId };
        var notifications = profile.NotificationSettings ?? new NotificationSettings { UserId = profile.UserId };
        var privacy = profile.PrivacySettings ?? new PrivacySettings { UserId = profile.UserId };

        return OperationResultFactory.Success(new SettingsResponse(
            new UserPreferencesDto(
                preferences.Language,
                preferences.Theme,
                preferences.WeightUnit,
                preferences.HeightUnit,
                preferences.DistanceUnit),
            new NotificationSettingsDto(
                notifications.WorkoutReminders,
                notifications.MealReminders,
                notifications.AchievementAlerts,
                notifications.EmailNotifications,
                notifications.WeeklyReports,
                notifications.PushNotifications),
            new PrivacySettingsDto(
                privacy.ProfileVisibility,
                privacy.ShowProgressToFriends,
                privacy.AllowDataSharing)));
    }
}
