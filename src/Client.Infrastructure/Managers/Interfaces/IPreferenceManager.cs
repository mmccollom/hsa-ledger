using HsaLedger.Shared.Settings;
using HsaLedger.Shared.Wrapper;

namespace HsaLedger.Client.Infrastructure.Managers.Interfaces;

public interface IPreferenceManager
{
    Task SetPreference(IPreference preference);

    Task<IPreference> Preference();

    Task<IResult> ChangeLanguageAsync(string languageCode);
}