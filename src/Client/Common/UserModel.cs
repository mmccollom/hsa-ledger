using HsaLedger.Application.Responses.Projections;

namespace HsaLedger.Client.Common;

public class UserModel
{
    public required string UserId { get; set; }
    public required string Username { get; set; }
    public required bool IsEnabled { get; set; }
    public required IEnumerable<RoleModel> Roles { get; set; }
    
    public string GetRoles()
    {
        var roles = string.Join(",", Roles.Select(t => t.RoleName).OrderBy(x => x));
        return roles.Length > 100 ? string.Concat(roles.AsSpan(0, 100), "...") : roles;
    }

    public static UserModel FromUserResponse(UserResponse response)
    {
        return new UserModel()
        {
            UserId = response.UserId,
            Username = response.Username,
            IsEnabled = response.IsEnabled,
            Roles = new List<RoleModel>(response.Roles
                .Where(x => !string.IsNullOrEmpty(x.RoleId))
                .Select(RoleModel.FromRoleResponse)),
        };
    }
}