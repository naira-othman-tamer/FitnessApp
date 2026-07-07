using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using WorkoutService.Domain.Enums;

namespace WorkoutService.Domain.Entities
{
    public class Workout : BaseEntity
    {
        public string Name { get; set; } = default!;
        public WorkoutCategory Category { get; set; }
        public int DurationInMinutes { get; set; }
        public int CaloriesBurn { get; set; }
        public string? ImageUrl { get; set; }
        public bool IsPremium { get; set; }
        public ICollection<WorkoutExercise> WorkoutExercises { get; set; } = new List<WorkoutExercise>();

        public Workout() { }

    }

    public class WorkoutConfiguration : IEntityTypeConfiguration<Workout>
    {
        public void Configure(EntityTypeBuilder<Workout> builder)
        {
            builder.ToTable("Workouts");

            builder.HasKey(x => x.Id);

            builder.HasQueryFilter(x => !x.IsDeleted);

            builder.HasIndex(x => x.Category);

            builder.Property(x => x.Name)
                   .HasMaxLength(150)
                   .IsRequired();

            builder.Property(x => x.Category)
                   .HasConversion<string>()
                   .HasMaxLength(30)
                   .IsRequired();

            builder.Property(x => x.DurationInMinutes)
                   .IsRequired();

            builder.Property(x => x.CaloriesBurn)
                   .IsRequired(false);

            builder.Property(x => x.ImageUrl)
                   .HasMaxLength(300);

            builder.Property(x => x.IsPremium)
                   .HasDefaultValue(false)
                   .IsRequired(false);

            builder.HasMany(x => x.WorkoutExercises)
                   .WithOne()
                   .HasForeignKey(x => x.WorkoutId)
                   .OnDelete(DeleteBehavior.Cascade);
            // deleting a Workout removes its exercise mappings — safe since WorkoutExercise is just a join
        }
    }
}
