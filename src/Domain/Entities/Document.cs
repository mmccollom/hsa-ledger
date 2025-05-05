using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using HsaLedger.Domain.Common.Persistence;

namespace HsaLedger.Domain.Entities;

public class Document : BaseAuditableEntity
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int DocumentId { get; set; }

    public int TransactionId { get; set; }
    public virtual Transaction Transaction { get; set; } = null!;
    
    public required string Fullname { get; set; }
    public required string Name { get; set; }
    public required string Extension { get; set; }
    public required byte[] Content { get; set; }
}