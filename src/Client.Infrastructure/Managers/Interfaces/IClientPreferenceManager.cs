using MudBlazor;

namespace HsaLedger.Client.Infrastructure.Managers.Interfaces;

public interface IClientPreferenceManager : IPreferenceManager
{
    Task<MudTheme> CurrentThemeAsync();
    Task<bool> ToggleLayoutDirection();
    Task<bool> ToggleDarkModeAsync();
    Task<bool> IsRtl();
    Task<bool> IsDarkMode();
}