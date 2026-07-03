using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace FCE.Domain.Entities
{
    public class UserPlanHistory : BaseEntity
    {
        public Guid UserId { get; set; }
        public int PlanId { get; set; } // Assuming this is the ID of the plan assigned to the user
        public DateTime? EndedAt { get; set; }
        public string? ResonForChange { get; set; }
    }

    public class UserPlanHistoryConfiguration : IEntityTypeConfiguration<UserPlanHistory>
    {
        public void Configure(EntityTypeBuilder<UserPlanHistory> builder)
        {
            builder.ToTable("UserPlanHistory");

            builder.HasKey(x => x.Id);

            builder.HasIndex(x => x.UserId); // non-unique — many history rows per user, by design

            builder.Property(x => x.UserId)
                   .IsRequired();

            builder.Property(x => x.ResonForChange)
                   .HasMaxLength(255)
                   .IsRequired(false);

            builder.Property(x => x.EndedAt)
                   .IsRequired(false);
        }
    }
}

