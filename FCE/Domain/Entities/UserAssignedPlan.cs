using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace FCE.Domain.Entities
{
    public class UserAssignedPlan  : BaseEntity
    {
        public Guid userId { get; set; }
        public int ExternalPlanId { get; set; }
        //public DateTime AssignedAt { get; set; } >> CreatedAt
        public bool IsActive { get; set; }
    }

    public class UserAssignedPlanConfiguration : IEntityTypeConfiguration<UserAssignedPlan>
    {
        public void Configure(EntityTypeBuilder<UserAssignedPlan> builder)
        {
            builder.ToTable("UserAssignedPlans");

            builder.HasKey(x => x.Id);

            builder.HasIndex(x => x.userId).IsUnique();

            builder.Property(x => x.userId)
                   .IsRequired();

            builder.Property(x => x.ExternalPlanId)
                   .HasMaxLength(100)
                   .IsRequired();

            builder.Property(x => x.IsActive)
                   .HasDefaultValue(true);
        }
    }

}
