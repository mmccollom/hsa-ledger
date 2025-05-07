using Blazored.LocalStorage;
using HsaLedger.Client.Infrastructure.Managers.Interfaces;
using HsaLedger.Client.Infrastructure.Settings;
using HsaLedger.Shared.Common.Constants.Storage;
using HsaLedger.Shared.Settings;
using HsaLedger.Shared.Wrapper;
using MudBlazor;

namespace HsaLedger.Client.Infrastructure.Managers;

public class ClientPreferenceManager : IClientPreferenceManager
{
    private readonly ILocalStorageService _localStorageService;

        public ClientPreferenceManager(
            ILocalStorageService localStorageService)
        {
            _localStorageService = localStorageService;
        }

        public async Task<bool> ToggleDarkModeAsync()
        {
            var preference = await Preference() as ClientPreference;
            if (preference != null)
            {
                preference.IsDarkMode = !preference.IsDarkMode;
                await SetPreference(preference);
                return preference.IsDarkMode;
            }

            return false;
        }
        public async Task<bool> ToggleLayoutDirection()
        {
            var preference = await Preference() as ClientPreference;
            if (preference != null)
            {
                preference.IsRtl = !preference.IsRtl;
                await SetPreference(preference);
                return preference.IsRtl;
            }
            return false;
        }

        public async Task<IResult> ChangeLanguageAsync(string languageCode)
        {
            var preference = await Preference() as ClientPreference;
            if (preference != null)
            {
                preference.LanguageCode = languageCode;
                await SetPreference(preference);
                return new Result
                {
                    Succeeded = true,
                    Messages = new List<string> { "Client Language has been changed" }
                };
            }

            return new Result
            {
                Succeeded = false,
                Messages = new List<string> { "Failed to get client preferences" }
            };
        }

        public async Task<MudTheme> CurrentThemeAsync()
        {
            return await Task.FromResult(BlazorHeroTheme.DefaultTheme);
        }
        
        public async Task<bool> IsRtl()
        {
            var preference = await Preference() as ClientPreference;
            if (preference == null)
            {
                return preference is { IsRtl: true };
            }
            
            return preference.IsRtl;
        }
        
        public async Task<bool> IsDarkMode()
        {
            var preference = await Preference() as ClientPreference;
            if (preference == null)
            {
                return preference is { IsDarkMode: false };
            }

            return preference.IsDarkMode;
        }

        public async Task<IPreference> Preference()
        {
            return await _localStorageService.GetItemAsync<ClientPreference>(StorageConstants.Local.Preference) ?? new ClientPreference();
        }

        public async Task SetPreference(IPreference preference)
        {
            await _localStorageService.SetItemAsync(StorageConstants.Local.Preference, preference as ClientPreference);
        }
}