using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using ProfileService.Domain.Entities;

namespace ProfileService.Data.Configurations;

public sealed class UserPreferencesConfiguration : IEntityTypeConfiguration<UserPreferences>
{
    public void Configure(EntityTypeBuilder<UserPreferences> builder)
    {
        builder.HasKey(x => x.UserId);
        builder.Property(x => x.Language).HasMaxLength(10).HasDefaultValue("en");
        builder.Property(x => x.Theme).HasMaxLength(15).HasDefaultValue("light");
        builder.Property(x => x.WeightUnit).HasMaxLength(5).HasDefaultValue("kg");
        builder.Property(x => x.HeightUnit).HasMaxLength(5).HasDefaultValue("cm");
        builder.Property(x => x.DistanceUnit).HasMaxLength(5).HasDefaultValue("km");
        builder.HasOne(x => x.UserProfile)
            .WithOne(x => x.Preferences)
            .HasForeignKey<UserPreferences>(x => x.UserId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
