using AuthenticationService.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace AuthenticationService.Data.Configurations;

public sealed class LoginAttemptConfiguration : IEntityTypeConfiguration<LoginAttempt>
{
    public void Configure(EntityTypeBuilder<LoginAttempt> builder)
    {
        builder.ToTable("LoginAttempts");
        builder.HasKey(x => x.Id);
        builder.Property(x => x.Id).ValueGeneratedNever();
        builder.Property(x => x.Email).HasMaxLength(255).IsUnicode(false).IsRequired();
        builder.HasIndex(x => new { x.Email, x.AttemptedAt });
        builder.Property(x => x.IpAddress).HasMaxLength(45).IsUnicode(false).IsRequired();
        builder.Property(x => x.AttemptedAt).HasPrecision(0);
    }
}
