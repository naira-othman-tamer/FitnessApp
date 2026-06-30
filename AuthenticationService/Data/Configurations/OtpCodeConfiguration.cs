using AuthenticationService.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace AuthenticationService.Data.Configurations;

public sealed class OtpCodeConfiguration : IEntityTypeConfiguration<OtpCode>
{
    public void Configure(EntityTypeBuilder<OtpCode> builder)
    {
        builder.ToTable("OtpCodes");
        builder.HasKey(x => x.Id);
        builder.Property(x => x.Id).ValueGeneratedNever();
        builder.Property(x => x.Email).HasMaxLength(255).IsUnicode(false).IsRequired();
        builder.HasIndex(x => x.Email);
        builder.Property(x => x.Code).HasMaxLength(64).IsUnicode(false).IsRequired();
        builder.Property(x => x.ExpiresAt).HasPrecision(0);
        builder.Property(x => x.CreatedAt).HasPrecision(0);
    }
}
