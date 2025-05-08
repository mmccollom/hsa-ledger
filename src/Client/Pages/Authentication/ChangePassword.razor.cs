using Blazored.FluentValidation;
using HsaLedger.Application.Requests.Identity;
using MudBlazor;

namespace HsaLedger.Client.Pages.Authentication;

public partial class ChangePassword
{
    private FluentValidationValidator? _fluentValidationValidator;
    private readonly ChangePasswordRequest _model = new()
    {
        Password = string.Empty,
        NewPassword = string.Empty,
        ConfirmNewPassword = string.Empty,
    };

    protected override async Task OnInitializedAsync()
    {
        var state = await _stateProvider.GetAuthenticationStateAsync();
        var isAuthenticated = state.User.Identity?.IsAuthenticated;
        if (!isAuthenticated.GetValueOrDefault(false))
        {
            _navigationManager.NavigateTo("/");
        }
    }

    private async Task SubmitAsync()
    {
        var result = await _authenticationManager.ChangePassword(_model);
        if (!result.Succeeded && result.Messages != null)
        {
            foreach (var message in result.Messages)
            {
                _snackBar.Add(message, Severity.Error);
            }
        }
        else
        {
            _snackBar.Add("Your password was changed successfully!", Severity.Success);
        }
        
        _navigationManager.NavigateTo("/");
    }

    private bool _passwordVisibility;
    private bool _newPasswordVisibility;
    private bool _confirmPasswordVisibility;
    private InputType _passwordInput = InputType.Password;
    private InputType _newPasswordInput = InputType.Password;
    private InputType _confirmPasswordInput = InputType.Password;
    private string _passwordInputIcon = Icons.Material.Filled.VisibilityOff;
    private string _newPasswordInputIcon = Icons.Material.Filled.VisibilityOff;
    private string _confirmPasswordInputIcon = Icons.Material.Filled.VisibilityOff;

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
    void ToggleNewPasswordVisibility()
    {
        if (_newPasswordVisibility)
        {
            _newPasswordVisibility = false;
            _newPasswordInputIcon = Icons.Material.Filled.VisibilityOff;
            _newPasswordInput = InputType.Password;
        }
        else
        {
            _newPasswordVisibility = true;
            _newPasswordInputIcon = Icons.Material.Filled.Visibility;
            _newPasswordInput = InputType.Text;
        }
    }
    void ToggleConfirmPasswordVisibility()
    {
        if (_confirmPasswordVisibility)
        {
            _confirmPasswordVisibility = false;
            _confirmPasswordInputIcon = Icons.Material.Filled.VisibilityOff;
            _confirmPasswordInput = InputType.Password;
        }
        else
        {
            _confirmPasswordVisibility = true;
            _confirmPasswordInputIcon = Icons.Material.Filled.Visibility;
            _confirmPasswordInput = InputType.Text;
        }
    }
}