using Blazored.FluentValidation;
using HsaLedger.Application.Requests;
using MudBlazor;

namespace HsaLedger.Client.Pages.Authentication;

public partial class Login
{
    private FluentValidationValidator? _fluentValidationValidator;
    private readonly LoginRequest _loginRequest = new()
    {
        Username = string.Empty,
        Password = string.Empty
    };

    protected override async Task OnInitializedAsync()
    {
        var state = await _stateProvider.GetAuthenticationStateAsync();
        var isAuthenticated = state.User.Identity?.IsAuthenticated;
        if (isAuthenticated.GetValueOrDefault(false))
        {
            _navigationManager.NavigateTo("/");
        }
    }

    private async Task SubmitAsync()
    {
        var result = await _authenticationManager.Login(_loginRequest);
        if (result is { Succeeded: false, Messages: { } })
        {
            if (result.Messages[0].Equals("Update your temporary password"))
            {
                _navigationManager.NavigateTo($"/changetemporarypassword?username={_loginRequest.Username}&token={result.Messages[1]}");
                return;
            }
            foreach (var message in result.Messages)
            {
                _snackBar.Add(message, Severity.Error);
            }
        }
    }

    private bool _passwordVisibility;
    private InputType _passwordInput = InputType.Password;
    private string _passwordInputIcon = Icons.Material.Filled.VisibilityOff;

    void TogglePasswordVisibility()
    {
        if (_passwordVisibility)
        {
            _passwordVisibility = false;
            _passwordInputIcon = Icons.Material.Filled.VisibilityOff;
            _passwordInput = InputType.Password;
        }
        else
        {
            _passwordVisibility = true;
            _passwordInputIcon = Icons.Material.Filled.Visibility;
            _passwordInput = InputType.Text;
        }
    }
}