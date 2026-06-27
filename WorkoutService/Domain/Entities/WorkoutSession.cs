using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using WorkoutService.Domain.Enums;

namespace WorkoutService.Domain.Entities
{
    public class WorkoutSession : BaseEntity
    {
        //SessionId (unique token), UserId, WorkoutId, StartedAt, CompletedAt?, Status
        public string SessionId { get; set; } = default!;
        public Guid UserId { get; set; }
        public int WorkoutId { get; set; }
        public DateTime StartedAt { get; set; } = DateTime.Now;
        public DateTime? CompletedAt { get; set; }
        public SessionStatus sessionStatus { get; set; } = SessionStatus.Active;

        private WorkoutSession() { }

        public static WorkoutSession Start(Guid userId, int workoutId)
        {
            return new WorkoutSession
            {
                SessionId = Guid.NewGuid().ToString(),
                UserId = userId,
                WorkoutId = workoutId
            };
        }
    }

    public class WorkoutSessionConfiguration : IEntityTypeConfiguration<WorkoutSession>
    {
        public void Configure(EntityTypeBuilder<WorkoutSession> builder)
        {
            builder.ToTable("WorkoutSessions");

            builder.Property(s => s.SessionId)
                .IsRequired()
                .HasMaxLength(100);

            builder.HasIndex(s => s.SessionId).IsUnique();
            builder.HasIndex(s => s.UserId);

            builder.Property(s => s.sessionStatus)
                .HasConversion<string>()
                .HasMaxLength(20)
                .IsRequired();

            builder.HasOne<Workout>()
                .WithMany()
                .HasForeignKey(s => s.WorkoutId)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}
