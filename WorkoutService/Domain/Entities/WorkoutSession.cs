using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using WorkoutService.Domain.Entities;
using WorkoutService.Domain.Enums;

namespace WorkoutService.Domain.Entities
{
    public class WorkoutSession : BaseEntity
    {
        public Guid UserId { get; private set; }
        public int WorkoutId { get; private set; }
        public Workout Workout { get; private set; } = default!;
        public DateTime StartedAt { get; private set; } = DateTime.UtcNow;
        public DateTime? CompletedAt { get; private set; }
        public SessionStatus Status { get; private set; } = SessionStatus.Active;

        private WorkoutSession() { }

        public static WorkoutSession Start(Guid userId, int workoutId)
            => new WorkoutSession
            {
                UserId = userId,
                WorkoutId = workoutId,
                Status = SessionStatus.Active
            };

        public void Complete()
        {
            if (Status != SessionStatus.Active)
                throw new InvalidOperationException("Only an active session can be completed.");

            Status = SessionStatus.Completed;
            CompletedAt = DateTime.UtcNow;
        }

        public void Abandon()
        {
            if (Status != SessionStatus.Active)
                throw new InvalidOperationException("Only an active session can be abandoned.");

            Status = SessionStatus.Abandoned;
            CompletedAt = DateTime.UtcNow;
        }
    }
}

    public class WorkoutSessionConfiguration : IEntityTypeConfiguration<WorkoutSession>
    {
    public void Configure(EntityTypeBuilder<WorkoutSession> builder)
    {
        builder.ToTable("WorkoutSessions");

        builder.HasKey(x => x.Id);

        builder.HasQueryFilter(x => !x.IsDeleted);

        builder.HasIndex(x => x.UserId); // "my sessions" lookups

        builder.Property(x => x.UserId)
               .IsRequired();

        builder.Property(x => x.WorkoutId)
               .IsRequired();

        builder.HasOne(x => x.Workout)
               .WithMany()
               .HasForeignKey(x => x.WorkoutId)
               .OnDelete(DeleteBehavior.Restrict);

        builder.Property(x => x.StartedAt)
               .IsRequired();

        builder.Property(x => x.CompletedAt);

        builder.Property(x => x.Status)
               .HasConversion<string>()
               .HasMaxLength(20)
               .IsRequired();
    }
}


