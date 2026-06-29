using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using ProfileService.Domain.Entities;

namespace ProfileService.Data.Configurations;

public sealed class PrivacySettingsConfiguration : IEntityTypeConfiguration<PrivacySettings>
{
    public void Configure(EntityTypeBuilder<PrivacySettings> builder)
    {
        builder.HasKey(x => x.UserId);
        builder.Property(x => x.ProfileVisibility).HasMaxLength(20).HasDefaultValue("private");
        builder.Property(x => x.ShowProgressToFriends).HasDefaultValue(false);
        builder.Property(x => x.AllowDataSharing).HasDefaultValue(false);
        builder.HasOne(x => x.UserProfile)
            .WithOne(x => x.PrivacySettings)
            .HasForeignKey<PrivacySettings>(x => x.UserId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
