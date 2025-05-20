using System.Linq.Expressions;
using HsaLedger.Domain.Common.Model;

namespace HsaLedger.Application.Responses.Projections;

public class UserResponse
{
    public required string UserId { get; set; }
    public required string Username { get; set; }
    public required bool IsEnabled { get; set; }
    public required List<RoleResponse> Roles { get; set; }
    
    public static Expression<Func<AppUser, UserResponse>> Projection
    {
        get
        {
            return x => new UserResponse
            {
                UserId = x.UserId,
                Username = x.Username,
                IsEnabled = x.IsEnabled,
                Roles = x.Roles.AsQueryable().Select(RoleResponse.Projection).ToList()
            };
        }
    }
    
    public static UserResponse FromEntity(AppUser entity)
    {
        return Projection.Compile().Invoke(entity);
    }
}