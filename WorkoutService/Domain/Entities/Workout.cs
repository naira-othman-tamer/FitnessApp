using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using WorkoutService.Domain.Enums;

namespace WorkoutService.Domain.Entities
{
    public class Workout : BaseEntity
    {
        public string Name { get; private set; } = default!;
        public WorkoutCategory Category { get; private set; }
        public Difficulty Difficulty { get; private set; }
        public int DurationInMinutes { get; private set; }
        public int CaloriesBurn { get; private set; }
        public string? ImageUrl { get; private set; }
        public bool IsPremium { get; private set; }

        public ICollection<WorkoutExercise> WorkoutExercises { get; private set; } = new List<WorkoutExercise>();

        private Workout() { }

        public static Workout Create(
            string name,
            Difficulty difficulty,
            int durationInMinutes,
            int caloriesBurn,
            WorkoutCategory? category = null,
            string? imageUrl = null,
            bool isPremium = false)
        {
            if (string.IsNullOrWhiteSpace(name))
                throw new ArgumentException("Name is required.");
            if (durationInMinutes <= 0)
                throw new ArgumentException("DurationInMinutes must be greater than zero.");
            if (caloriesBurn < 0)
                throw new ArgumentException("CaloriesBurn cannot be negative.");

            return new Workout
            {
                Name = name,
                Difficulty = difficulty,
                DurationInMinutes = durationInMinutes,
                CaloriesBurn = caloriesBurn,
                Category = category ?? WorkoutCategory.Unknown,
                ImageUrl = imageUrl,
                IsPremium = isPremium
            };
        }
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

            builder.Property(x => x.Difficulty)
                   .HasConversion<string>()
                   .HasMaxLength(20)
                   .IsRequired();

            builder.Property(x => x.DurationInMinutes)
                   .IsRequired();

            builder.Property(x => x.CaloriesBurn)
                   .IsRequired();

            builder.Property(x => x.ImageUrl)
                   .HasMaxLength(300);

            builder.Property(x => x.IsPremium)
                   .HasDefaultValue(false)
                   .IsRequired();

            builder.HasMany(x => x.WorkoutExercises)
                   .WithOne()
                   .HasForeignKey(x => x.WorkoutId)
                   .OnDelete(DeleteBehavior.Cascade);
            // deleting a Workout removes its exercise mappings — safe since WorkoutExercise is just a join
        }
    }
}
