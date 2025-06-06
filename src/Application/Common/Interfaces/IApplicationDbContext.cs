using HsaLedger.Domain.Common.Model;
using HsaLedger.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;

namespace HsaLedger.Application.Common.Interfaces;

public interface IApplicationDbContext
{
    DatabaseFacade DatabaseFacade { get; }
    DbSet<Document> Documents { get; }
    DbSet<Person> Persons { get; }
    DbSet<Provider> Providers { get; }
    DbSet<Transaction> Transactions { get; }
    DbSet<TransactionType> TransactionTypes { get; }
    DbSet<AspNetUserRefreshToken> AspNetUserRefreshTokens { get; }
    Task<IEnumerable<AppUser>> GetUsers();
    Task<IEnumerable<AppRole>> GetRoles();
    Task<int> SaveChangesAsync(CancellationToken cancellationToken);
}