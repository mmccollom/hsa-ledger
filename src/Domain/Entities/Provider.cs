using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using HsaLedger.Domain.Common.Persistence;

namespace HsaLedger.Domain.Entities;

public class Provider : BaseAuditableEntity
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int ProviderId { get; set; }
    
    public required string Name { get; set; }
    
    public virtual ICollection<Transaction> Transactions { get; set; } = new List<Transaction>();
    public virtual ICollection<TransactionType> TransactionTypes { get; set; } = new List<TransactionType>();
}