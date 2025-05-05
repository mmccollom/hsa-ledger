using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using HsaLedger.Domain.Common.Persistence;

namespace HsaLedger.Domain.Entities;

public class Transaction : BaseAuditableEntity
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int TransactionId { get; set; }

    public int TransactionTypeId { get; set; }
    public virtual TransactionType TransactionType { get; set; } = null!;
    public int ProviderId { get; set; }
    public virtual Provider Provider { get; set; } = null!;
    public int? PersonId { get; set; }
    public virtual Person? Person { get; set; }
    
    public DateTime Date { get; set; }
    public int Amount { get; set; }
    public bool IsPaid { get; set; }
    public bool IsHsaWithdrawn { get; set; }
    public bool IsAudited { get; set; }

    public virtual ICollection<Document> Documents { get; set; } = new List<Document>();
}