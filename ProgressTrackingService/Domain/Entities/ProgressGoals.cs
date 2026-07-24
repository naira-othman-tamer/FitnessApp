using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ProgressTrackingService.Domain.Entities;

public class ProgressGoals : BaseEntity
{
   public Guid UserId { get; set; }//(Guid, unique — one active goal)
   public double? TargetWeightKg { get; set; }//(optional)
   public DateTime? TargetDate { get; set; }//(optional)
   public DateTime? CompletedAt { get; set; }// (optional)
}

public class ProgressGoalsConfiguration : IEntityTypeConfiguration<ProgressGoals>
{
    public void Configure(EntityTypeBuilder<ProgressGoals> builder)
    {
        builder.ToTable("ProgressGoals");

        builder.HasKey(x => x.Id);

        builder.Property(x => x.UserId).IsRequired();
        builder.Property(x => x.TargetWeightKg).IsRequired(false);
        builder.Property(x => x.TargetDate).IsRequired(false);
        builder.Property(x => x.CompletedAt).IsRequired(false);

        builder.Property(x => x.CreatedAt).IsRequired();
        builder.Property(x => x.IsDeleted).HasDefaultValue(false);

        // Exactly one non-deleted, unfinished goal per user.
        builder.HasIndex(x => x.UserId)
            .IsUnique()
            .HasFilter("[IsDeleted] = 0 AND [CompletedAt] IS NULL");

        builder.HasQueryFilter(x => !x.IsDeleted);
    }
}
