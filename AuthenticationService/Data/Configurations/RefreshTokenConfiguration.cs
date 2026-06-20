using AuthenticationService.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace AuthenticationService.Data.Configurations;

public sealed class RefreshTokenConfiguration : IEntityTypeConfiguration<RefreshToken>
{
    public void Configure(EntityTypeBuilder<RefreshToken> builder)
    {
        builder.ToTable("RefreshTokens");
        builder.HasKey(x => x.Id);
        builder.Property(x => x.Id).ValueGeneratedNever();
        builder.Property(x => x.Token).HasMaxLength(512).IsUnicode(false).IsRequired();
        builder.HasIndex(x => x.Token).IsUnique();
        builder.Property(x => x.ExpiresAt).HasPrecision(0);
        builder.Property(x => x.CreatedAt).HasPrecision(0);
        builder.Property(x => x.RevokedAt).HasPrecision(0);
        builder.HasOne(x => x.User).WithMany(x => x.RefreshTokens).HasForeignKey(x => x.UserId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
