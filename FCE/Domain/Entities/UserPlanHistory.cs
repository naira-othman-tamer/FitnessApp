using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace FCE.Domain.Entities
{
    public class UserPlanHistory : BaseEntity
    {
        public int UserId { get; set; }
        public int ExternalPlanId { get; set; }
        //public DateTime AssignedAt { get; set; } >> CreatedAt
        public DateTime EndedAt { get; set; }
        public string? ResonForChange { get; set; }
    }

    public class UserPlanHistoryConfiguration : IEntityTypeConfiguration<UserPlanHistory>
    {
        public void Configure(EntityTypeBuilder<UserPlanHistory> builder)
        {
            builder.ToTable("UserPlanHistory");

            builder.HasKey(x => x.Id);

            builder.HasIndex(x => x.UserId);

            builder.Property(x => x.UserId)
                   .IsRequired();

            builder.Property(x => x.ExternalPlanId)
                   .HasMaxLength(100)
                   .IsRequired();

            builder.Property(x => x.ResonForChange)
                   .HasMaxLength(255)
                   .IsRequired(false);

            builder.Property(x => x.EndedAt)
                   .IsRequired(false);
        }
    }
}
