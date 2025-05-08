using MudBlazor;

namespace HsaLedger.Client.Shared;

public partial class MainLayout : IDisposable
{
    private MudTheme? _currentTheme;
    private bool _isDarkMode;

    protected override async Task OnInitializedAsync()
    {
        _currentTheme = await _clientPreferenceManager.CurrentThemeAsync();
        _isDarkMode = await _clientPreferenceManager.IsDarkMode();
        _interceptor.RegisterEvent();
    }

    private async Task DarkMode()
    {
        var isDarkMode = await _clientPreferenceManager.ToggleDarkModeAsync();
        _isDarkMode = isDarkMode;
    }

    public void Dispose()
    {
        _interceptor.DisposeEvent();
    }
}