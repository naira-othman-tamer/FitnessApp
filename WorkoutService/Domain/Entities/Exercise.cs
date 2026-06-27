using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using WorkoutService.Domain.Enums;

namespace WorkoutService.Domain.Entities
{
    public class Exercise : BaseEntity
    {
        public string Name { get; set; } = default!;
       public string? Description { get; set; } 
       public string? VideoUrl {  get; set; }
       public ICollection<string> TargetMuscles { get; set; }=new List<string>();
       public ICollection<string> EquipmentNeeded { get; set; }=new List<string>();
       public Difficulty ExerciseDifficulty { get; set; }
    }

    public class ExerciseConfiguration : IEntityTypeConfiguration<Exercise>
    {
        public void Configure(EntityTypeBuilder<Exercise> builder)
        {
            builder.ToTable("Exercises");

            builder.Property(e => e.Name)
                .IsRequired()
                .HasMaxLength(100);

            builder.Property(e => e.Description)
                .HasMaxLength(1000);

            builder.Property(e => e.VideoUrl)
                .HasMaxLength(500);

            builder.Property(e => e.TargetMuscles)
                .HasColumnType("nvarchar(max)");

            builder.Property(e => e.EquipmentNeeded)
                .HasColumnType("nvarchar(max)");

            builder.Property(e => e.ExerciseDifficulty)
                .HasConversion<string>()
                .HasMaxLength(20)
                .IsRequired();
        }
    }
}
