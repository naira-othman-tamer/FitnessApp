using AuthenticationService.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace AuthenticationService.Data.Configurations;

public sealed class UserConfiguration : IEntityTypeConfiguration<ApplicationUser>
{
    public void Configure(EntityTypeBuilder<ApplicationUser> builder)
    {
        builder.ToTable("Users");
        builder.HasKey(x => x.Id);
        builder.Property(x => x.Id).ValueGeneratedNever();
        builder.Property(x => x.Email).HasMaxLength(255).IsUnicode(false).IsRequired();
        builder.Property(x => x.NormalizedEmail).HasMaxLength(255).IsUnicode(false);
        builder.Property(x => x.PasswordHash).HasMaxLength(512).IsUnicode(false);
        builder.Property(x => x.CreatedAt).HasPrecision(0);
    }
}
