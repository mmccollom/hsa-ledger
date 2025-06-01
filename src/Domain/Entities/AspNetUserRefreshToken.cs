using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using HsaLedger.Domain.Common.Persistence;

namespace HsaLedger.Domain.Entities;

public class AspNetUserRefreshToken : BaseAuditableEntity
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int AspNetUserRefreshTokenId { get; set; }
    
    public required string UserId { get; set; }
    
    public required string Hash { get; set; }
    public required string Salt { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime ExpiresAt { get; set; }
    public string? Device { get; set; }
    public string? IpAddress { get; set; }
    public bool IsRevoked { get; set; }
}