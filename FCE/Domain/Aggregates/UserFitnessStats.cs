using FCE.Domain.Entities;
using FCE.Domain.Enums;
using FCE.Domain.ValueObject;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace FCE.Domain.Aggregates
{
    public class UserFitnessStats : BaseEntity //stage1 [Stores raw physical input variables]
    {
        public Guid userId { get; set; }
        public PhysicalStats PhysicalStats { get; set; }
        public Goal goal { get; set; } //>> sent to workout to select workoutExcersise 
        public ActivityLevel activityLevel { get; set; }
        public bool IsActive { get; set; } = true;
        public int WorkoutDays { get; set; } //>> sent to workout to select workout plan
        //public int AssignedPlanId { get; set; }
    }

    public class UserFitnessStatsConfiguration : IEntityTypeConfiguration<UserFitnessStats>
    {
        public void Configure(EntityTypeBuilder<UserFitnessStats> builder)
        {
            builder.ToTable("UserFitnessStats");

            builder.HasKey(x => x.Id);

            builder.HasIndex(x => x.userId).IsUnique(); 

            builder.Property(x => x.userId)
                   .IsRequired();

            builder.Property(x => x.goal)
                   .HasConversion<string>()
                   .HasMaxLength(50)
                   .IsRequired();

            builder.Property(x => x.activityLevel)
                   .HasConversion<string>()
                   .HasMaxLength(50)
                   .IsRequired();

            builder.Property(x => x.WorkoutDays)
                   .IsRequired();

            builder.Property(x => x.IsActive)
                   .HasDefaultValue(true)
                   .IsRequired();

            builder.OwnsOne(x => x.PhysicalStats, p =>
            {
                p.Property(x => x.Weight).HasColumnName("Weight").IsRequired();
                p.Property(x => x.Height).HasColumnName("Height").IsRequired();
                p.Property(x => x.Age).HasColumnName("Age").IsRequired();
                p.Property(x => x.Gender)
                 .HasColumnName("Gender")
                 .HasConversion<string>()
                 .HasMaxLength(10)
                 .IsRequired();
            });
        }
    }
}
