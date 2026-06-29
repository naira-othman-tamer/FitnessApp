using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using WorkoutService.Domain.Enums;

namespace WorkoutService.Domain.Entities
{
    public class Exercise : BaseEntity
    {
        public string Name { get; set; } = default!;
       public string? Description { get; set; } 
       public string? ImageURL {  get; set; }
       public ICollection<string> TargetMuscles { get; set; }=new List<string>(); 
       public ICollection<EquipmentNeeded> EquipmentNeeded { get; set; } =  new List<EquipmentNeeded>();
       public Difficulty Difficulty { get; set; }
    }

    public class ExerciseConfiguration : IEntityTypeConfiguration<Exercise>
    {
        public void Configure(EntityTypeBuilder<Exercise> builder)
        {
            builder.ToTable("Exercises");

            builder.HasKey(x => x.Id);

            builder.Property(x => x.Name)
                   .HasMaxLength(150)
                   .IsRequired();

            builder.Property(x => x.Description)
                   .HasMaxLength(1000)
                   .IsRequired(false);

            builder.Property(x => x.ImageURL)
                   .HasMaxLength(300)
                   .IsRequired(false);

            builder.Property(x => x.Difficulty)
                   .HasConversion<string>()
                   .HasMaxLength(20)
                   .IsRequired();

            // List<string> <-> "Chest,Triceps"
            builder.Property(x => x.TargetMuscles)
                   .HasConversion(
                       v => string.Join(',', v),
                       v => v.Split(',', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries).ToList())
                   .HasMaxLength(500);

            // List<EquipmentNeeded> <-> "Machine,Dumbbells" (stored as names, not ints)
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


