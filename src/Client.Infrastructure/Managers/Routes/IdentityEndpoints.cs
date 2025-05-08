namespace HsaLedger.Client.Infrastructure.Managers.Routes;

internal static class IdentityEndpoints
{
    internal const string Register = "api/Identity/register";
    internal const string Login = "api/Identity/login";
    internal const string Refresh = "api/Identity/refresh";
    internal const string ChangePassword =  "api/Identity/changePassword";
    internal const string SetEnabled =   "api/Identity/setEnabled";
    internal const string SetRoles =   "api/Identity/setRoles";
}