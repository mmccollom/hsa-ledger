namespace HsaLedger.Shared.Common.Constants.Storage;

public static class StorageConstants
{
    public static class Local
    {
        public const string Preference = "clientPreference";

        public const string AuthToken = "authToken";
        public const string RefreshToken = "refreshToken";
        public const string AuthTokenExpiration = "tokenExpiration";
        public const string UserImageUrl = "userImageURL";
        public const string ChangeTemporaryPasswordGuid = "changeTemporaryPasswordGuid";
        public const string ActiveCustomer = "activeCustomer";
        public const string SelectedDashboardColumns = "selectedDashboardColumns";
    }

    public static class Server
    {
        public const string Preference = "serverPreference";
    }
}