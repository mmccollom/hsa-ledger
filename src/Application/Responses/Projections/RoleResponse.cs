using System.Linq.Expressions;
using HsaLedger.Domain.Common.Model;

namespace HsaLedger.Application.Responses.Projections;

public class RoleResponse
{
    public required string RoleId { get; set; }
    public required string RoleName { get; set; }
    public required string NormalizedName { get; set; }
    
    public static Expression<Func<AppRole, RoleResponse>> Projection
    {
        get
        {
            return x => new RoleResponse
            {
                RoleId = x.RoleId,
                RoleName = x.RoleName,
                NormalizedName = x.NormalizedName
            };
        }
    }
    
    public static RoleResponse FromEntity(AppRole entity)
    {
        return Projection.Compile().Invoke(entity);
    }
}