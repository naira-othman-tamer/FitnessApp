using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ProgressTrackingService.Domain.Entities
{
    public class BodyMeasurements : BaseEntity
    {
        public Guid UserId { get; }//(Guid, indexed)
        public double WeightKg { get; set; }//(required)
        public double? BodyFatPercent { get; set; }//(optional)
        public string? Notes { get; set; }
    }

    public class BodyMeasurementsConfiguration : IEntityTypeConfiguration<BodyMeasurements>
    {
        public void Configure(EntityTypeBuilder<BodyMeasurements> builder)
        {
            builder.ToTable("BodyMeasurements");

            builder.HasKey(x => x.Id);

            builder.Property(x => x.UserId).IsRequired();

            builder.Property(x => x.WeightKg).IsRequired();

            builder.Property(x => x.BodyFatPercent).IsRequired(false);

            builder.Property(x => x.Notes)
                .HasMaxLength(500)
                .IsRequired(false);

            builder.Property(x => x.CreatedAt).IsRequired();

            builder.Property(x => x.CreatedAt).IsRequired();
            builder.Property(x => x.IsDeleted).HasDefaultValue(false);

            builder.HasIndex(x => new { x.UserId, x.CreatedAt });

            builder.HasQueryFilter(x => !x.IsDeleted);
        }
    }
}
