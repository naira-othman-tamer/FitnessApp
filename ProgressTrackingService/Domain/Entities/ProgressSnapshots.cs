using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ProgressTrackingService.Domain.Entities;

public class ProgressSnapshots :BaseEntity
{
    public Guid UserId {  get; set; }
    public DateTime SnapShotDate {  get; set; }
    public double WeightKg {  get; set; }
    public int WorkoutsCompleted {  get; set; }
    public int CaloriesTarget {  get; set; }
}

public class ProgressSnapshotsConfiguration
    : IEntityTypeConfiguration<ProgressSnapshots>
{
    public void Configure(EntityTypeBuilder<ProgressSnapshots> builder)
    {
        builder.ToTable("ProgressSnapshots");

        builder.HasKey(x => x.Id);

        builder.Property(x => x.UserId)
            .IsRequired();

        builder.Property(x => x.SnapShotDate)
            .IsRequired();

        builder.Property(x => x.WeightKg)
            .IsRequired();

        builder.Property(x => x.WorkoutsCompleted)
            .IsRequired()
            .HasDefaultValue(0);

        builder.Property(x => x.CaloriesTarget)
            .IsRequired()
            .HasDefaultValue(0);

        builder.Property(x => x.CreatedAt)
            .IsRequired();

        builder.Property(x => x.IsDeleted)
            .IsRequired()
            .HasDefaultValue(false);

        // One snapshot per user for each day.
        builder.HasIndex(x => new { x.UserId, x.SnapShotDate })
            .IsUnique()
            .HasFilter("[IsDeleted] = 0");

        builder.HasQueryFilter(x => !x.IsDeleted);
    }
}
