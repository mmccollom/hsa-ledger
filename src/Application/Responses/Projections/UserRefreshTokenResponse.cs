using System.Linq.Expressions;

namespace HsaLedger.Application.Responses.Projections;

public class UserRefreshTokenResponse
{
    public int UserRefreshTokenId { get; set; }
    public required string UserId { get; set; }
    public required string Hash { get; set; }
    public required string Salt { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime ExpiresAt { get; set; }
    public string? Device { get; set; }
    public string? IpAddress { get; set; }
    public bool IsRevoked { get; set; }
    public DateTime CreatedTime { get; set; }
    public string? CreatedBy { get; set; }
    public DateTime? LastUpdatedTime { get; set; }
    public string? LastUpdatedBy { get; set; }
    public int LockId { get; set; }
    
    public static Expression<Func<Domain.Entities.UserRefreshToken, UserRefreshTokenResponse>> Projection
    {
        get
        {
            return x => new UserRefreshTokenResponse
            {
                UserRefreshTokenId = x.UserRefreshTokenId,
                UserId = x.UserId,
                Hash = x.Hash,
                Salt = x.Salt,
                CreatedAt = x.CreatedAt,
                ExpiresAt = x.ExpiresAt,
                Device = x.Device,
                IpAddress = x.IpAddress,
                IsRevoked = x.IsRevoked,
                CreatedTime = x.CreatedTime,
                CreatedBy = x.CreatedBy,
                LastUpdatedTime = x.LastUpdatedTime,
                LastUpdatedBy = x.LastUpdatedBy,
                LockId = x.LockId
            };
        }
    }
}