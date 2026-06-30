using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using WorkoutService.Domain.Enums;

namespace WorkoutService.Domain.Entities
{
    public class WorkoutSession : BaseEntity
    {
        public string SessionId { get; set; } = default!;
        public Guid UserId { get; set; }
        public int WorkoutPlanDayId { get; set; } // which day's workout this session is for
        public DateTime StartedAt { get; set; } = DateTime.UtcNow;
        public DateTime? CompletedAt { get; set; }
        public SessionStatus Status { get; set; } = SessionStatus.Active;

        private WorkoutSession() { }

        public static WorkoutSession Start(Guid userId, int workoutPlanDayId)
            => new WorkoutSession
            {
                SessionId = Guid.NewGuid().ToString(),
                UserId = userId,
                WorkoutPlanDayId = workoutPlanDayId
            };
    }

    public class WorkoutSessionConfiguration : IEntityTypeConfiguration<WorkoutSession>
    {
        public void Configure(EntityTypeBuilder<WorkoutSession> builder)
        {
            builder.ToTable("WorkoutSessions");

            builder.HasKey(x => x.Id);

            builder.HasIndex(x => x.SessionId).IsUnique(); // external lookup token, must be unique

            builder.HasIndex(x => x.UserId); // list "my sessions" lookups

            builder.Property(x => x.SessionId)
                   .HasMaxLength(50)
                   .IsRequired();

            builder.Property(x => x.UserId)
                   .IsRequired();

            builder.Property(x => x.WorkoutPlanDayId)
                   .IsRequired();

            builder.Property(x => x.StartedAt)
                   .IsRequired();

            builder.Property(x => x.CompletedAt)
                   .IsRequired(false);

            builder.Property(x => x.Status)
                   .HasConversion<string>()
                   .HasMaxLength(20)
                   .IsRequired();
        }
    }
}
