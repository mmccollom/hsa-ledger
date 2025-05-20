using HsaLedger.Application.Responses.Projections;

namespace HsaLedger.Client.Common.Models;

public class RoleModel : IEquatable<RoleModel>
{
    public RoleModel(string roleId)
    {
        RoleId = roleId;
    }

    public string RoleId { get; }
    public required string RoleName { get; set; }
    public required string NormalizedName { get; set; }
    
    public static RoleModel FromRoleResponse(RoleResponse response)
    {
        return new RoleModel(response.RoleId)
        {
            RoleName = response.RoleName,
            NormalizedName = response.NormalizedName,
        };
    }

    public bool Equals(RoleModel? other)
    {
        if (other is null) return false;
        if (ReferenceEquals(this, other)) return true;
        return RoleId == other.RoleId;
    }

    public override bool Equals(object? obj)
    {
        if (obj is null) return false;
        if (ReferenceEquals(this, obj)) return true;
        if (obj.GetType() != GetType()) return false;
        return Equals((RoleModel)obj);
    }

    public override int GetHashCode()
    {
        return HashCode.Combine(RoleId);
    }
}