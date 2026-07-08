using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using WorkoutService.Domain.Enums;

namespace WorkoutService.Domain.Entities
{
    public class Exercise : BaseEntity
    {
        public string Name { get; set; } = default!;
        public string? Description { get; set; }
        public string? ImageUrl { get; set; }
        public string? VideoUrl { get; set; }
        public Difficulty Difficulty { get; set; }
        public ICollection<MuscleGroup> TargetMuscles { get; set; } = new List<MuscleGroup>();
        public ICollection<EquipmentNeeded> EquipmentNeeded { get; set; } = new List<EquipmentNeeded>();

        public Exercise() { }

        public static Exercise Create(
            string name,
            Difficulty difficulty,
            IEnumerable<MuscleGroup> targetMuscles,
            IEnumerable<EquipmentNeeded> equipmentNeeded,
            string? description = null,
            string? imageUrl = null,
            string? videoUrl = null)
        {
            if (string.IsNullOrWhiteSpace(name))
                throw new ArgumentException("Name is required.");

            return new Exercise
            {
                Name = name,
                Difficulty = difficulty,
                TargetMuscles = targetMuscles.ToList(),
                EquipmentNeeded = equipmentNeeded.ToList(),
                Description = description,
                ImageUrl = imageUrl,
                VideoUrl = videoUrl
            };
        }
    }

    public class ExerciseConfiguration : IEntityTypeConfiguration<Exercise>
    {
        public void Configure(EntityTypeBuilder<Exercise> builder)
        {
            builder.ToTable("Exercises");

            builder.HasKey(x => x.Id);

            builder.HasQueryFilter(x => !x.IsDeleted);

            builder.Property(x => x.Name)
                   .HasMaxLength(150)
                   .IsRequired();

            builder.Property(x => x.Description)
                   .HasMaxLength(1000);

            builder.Property(x => x.ImageUrl)
                   .HasMaxLength(300);

            builder.Property(x => x.VideoUrl)
                   .HasMaxLength(300);

            builder.Property(x => x.Difficulty)
                   .HasConversion<string>()
                   .HasMaxLength(20)
                   .IsRequired();

            builder.Property(x => x.TargetMuscles)
                   .HasConversion(
                       v => string.Join(',', v.Select(m => m.ToString())),
                       v => v.Split(',', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries)
                             .Select(s => Enum.Parse<MuscleGroup>(s))
                             .ToList())
                   .HasMaxLength(300);

            builder.Property(x => x.EquipmentNeeded)
                   .HasConversion(
                       v => string.Join(',', v.Select(e => e.ToString())),
                       v => v.Split(',', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries)
                             .Select(s => Enum.Parse<EquipmentNeeded>(s))
                             .ToList())
                   .HasMaxLength(200);
        }
    }
}


