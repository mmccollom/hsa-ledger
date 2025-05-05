using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using HsaLedger.Domain.Common.Persistence;

namespace HsaLedger.Domain.Entities;

public class TransactionType : BaseAuditableEntity
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int TransactionTypeId { get; set; }

    public required string Code { get; set; }
    public required string Description { get; set; }

    public virtual ICollection<Provider> Providers { get; set; } = new List<Provider>(); 
    public virtual ICollection<Transaction> Transactions { get; set; } = new List<Transaction>(); 
}