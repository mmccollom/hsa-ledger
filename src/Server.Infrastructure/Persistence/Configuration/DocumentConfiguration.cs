using HsaLedger.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace HsaLedger.Server.Infrastructure.Persistence.Configuration;

public class DocumentConfiguration : IEntityTypeConfiguration<Document>
{
    public void Configure(EntityTypeBuilder<Document> builder)
    {
        builder.Property(x => x.Fullname).HasMaxLength(250).IsRequired();
        builder.Property(x => x.Name).HasMaxLength(200).IsRequired();
        builder.Property(x => x.Extension).HasMaxLength(50).IsRequired();
        builder.Property(x => x.Content).IsRequired();

        builder.HasOne(x => x.Transaction)
            .WithMany(x => x.Documents)
            .IsRequired();
    }
}