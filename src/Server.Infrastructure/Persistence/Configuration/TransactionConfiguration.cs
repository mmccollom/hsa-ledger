using HsaLedger.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace HsaLedger.Server.Infrastructure.Persistence.Configuration;

public class TransactionConfiguration : IEntityTypeConfiguration<Transaction>
{
    public void Configure(EntityTypeBuilder<Transaction> builder)
    {
        builder.Property(x => x.Date).IsRequired();
        builder.Property(x => x.Amount).IsRequired();
        builder.Property(x => x.IsPaid).HasDefaultValue(false).IsRequired();
        builder.Property(x => x.IsHsaWithdrawn).HasDefaultValue(false).IsRequired();
        builder.Property(x => x.IsAudited).HasDefaultValue(false).IsRequired();
        
        builder.HasOne(x => x.TransactionType)
            .WithMany(x => x.Transactions)
            .IsRequired();
        
        builder.HasOne(x => x.Provider)
            .WithMany(x => x.Transactions)
            .IsRequired();

        builder.HasOne(x => x.Person)
            .WithMany(x => x.Transactions);
    }
}