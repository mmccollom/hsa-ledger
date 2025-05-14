using System.Reflection;
using HsaLedger.Application.Common.Interfaces;
using HsaLedger.Domain.Common.Model;
using HsaLedger.Domain.Entities;
using HsaLedger.Server.Infrastructure.Identity;
using HsaLedger.Server.Infrastructure.Persistence.Interceptors;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;

namespace HsaLedger.Server.Infrastructure.Persistence;

public class ApplicationDbContext : IdentityDbContext<User, IdentityRole, string>, IApplicationDbContext
{
    private readonly AuditableEntitySaveChangesInterceptor _auditableEntitySaveChangesInterceptor;

    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options,
        AuditableEntitySaveChangesInterceptor auditableEntitySaveChangesInterceptor) : base(options)
    {
        _auditableEntitySaveChangesInterceptor = auditableEntitySaveChangesInterceptor;
    }
    
    public DatabaseFacade DatabaseFacade => Database;
    public DbSet<Document> Documents => Set<Document>();
    public DbSet<Person> Persons => Set<Person>();
    public DbSet<Provider> Providers => Set<Provider>();
    public DbSet<Transaction> Transactions => Set<Transaction>();
    public DbSet<TransactionType> TransactionTypes => Set<TransactionType>();
    
    public async Task<IEnumerable<AppUser>> GetUsers()
    {
        var data = from u in Users
            join ur in UserRoles on u.Id equals ur.UserId into urGroup
            from ur in urGroup.DefaultIfEmpty()
            join r in Roles on ur.RoleId equals r.Id into rGroup
            from r in rGroup.DefaultIfEmpty()
            select new
            {
                u.Id,
                u.UserName,
                u.IsEnabled,
                ur.RoleId,
                r.Name,
                r.NormalizedName
            }
            into d
            group d by new { d.Id, d.UserName, d.IsEnabled }
            into g
            select new AppUser
            {
                UserId = g.Key.Id,
                Username = g.Key.UserName,
                IsEnabled = g.Key.IsEnabled,
                Roles = g.Select(x => new AppRole
                {
                    RoleId = x.RoleId,
                    RoleName = x.Name,
                    NormalizedName = x.NormalizedName
                }).ToList()
            };
            
            return await data.ToListAsync();
    }
    
    public async Task<IEnumerable<AppRole>> GetRoles()
    {
        var data = from r in Roles
            select new AppRole
            {
                RoleId = r.Id,
                RoleName = r.Name,
                NormalizedName = r.NormalizedName
            };
            
            return await data.ToListAsync();
    }
    
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        modelBuilder.HasDefaultSchema("hsa");
        modelBuilder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());
    }
    
    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        optionsBuilder.AddInterceptors(_auditableEntitySaveChangesInterceptor);
    }
}