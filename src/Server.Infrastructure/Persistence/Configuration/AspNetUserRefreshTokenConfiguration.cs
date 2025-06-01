using HsaLedger.Domain.Entities;
using HsaLedger.Server.Infrastructure.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace HsaLedger.Server.Infrastructure.Persistence.Configuration;

public class AspNetUserRefreshTokenConfiguration : IEntityTypeConfiguration<AspNetUserRefreshToken>
{
    public void Configure(EntityTypeBuilder<AspNetUserRefreshToken> builder)
    {
        builder.Property(x => x.UserId).HasMaxLength(450).IsRequired();
        builder.Property(x => x.Hash).HasMaxLength(88).IsRequired();
        builder.Property(x => x.Salt).HasMaxLength(44).IsRequired();
        builder.Property(x => x.Device).HasMaxLength(150);
        builder.Property(x => x.IpAddress).HasMaxLength(45);
        builder.Property(x => x.CreatedAt).IsRequired();
        builder.Property(x => x.ExpiresAt).IsRequired();
        builder.Property(x => x.IsRevoked).HasDefaultValue(false).IsRequired();

        builder.HasOne<User>()
            .WithMany(x => x.RefreshTokens)
            .HasForeignKey(x => x.UserId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}