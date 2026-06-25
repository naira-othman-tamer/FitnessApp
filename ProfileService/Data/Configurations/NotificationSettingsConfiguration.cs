using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using ProfileService.Domain.Entities;

namespace ProfileService.Data.Configurations;

public sealed class NotificationSettingsConfiguration : IEntityTypeConfiguration<NotificationSettings>
{
    public void Configure(EntityTypeBuilder<NotificationSettings> builder)
    {
        builder.HasKey(x => x.UserId);
        builder.Property(x => x.WorkoutReminders).HasDefaultValue(true);
        builder.Property(x => x.MealReminders).HasDefaultValue(true);
        builder.Property(x => x.AchievementAlerts).HasDefaultValue(true);
        builder.Property(x => x.EmailNotifications).HasDefaultValue(true);
        builder.Property(x => x.WeeklyReports).HasDefaultValue(true);
        builder.Property(x => x.PushNotifications).HasDefaultValue(true);
        builder.HasOne(x => x.UserProfile)
            .WithOne(x => x.NotificationSettings)
            .HasForeignKey<NotificationSettings>(x => x.UserId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
