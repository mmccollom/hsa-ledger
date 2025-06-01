using HsaLedger.Client.Common.Extensions;
using HsaLedger.Client.Dialogs;
using HsaLedger.Shared.Common.Constants.Permission;
using Microsoft.AspNetCore.Components;
using MudBlazor;

namespace HsaLedger.Client.Shared;

public partial class MainBody
    {
        [Parameter]
        public RenderFragment? ChildContent { get; set; }

        [Parameter]
        public EventCallback OnDarkModeToggle { get; set; }

        [Parameter]
        public EventCallback<bool> OnRightToLeftToggle { get; set; }

        private bool _drawerOpen = true;

        private string? CurrentUserId { get; set; }
        private string? ImageDataUrl { get; set; }
        private string? FirstName { get; set; }
        private string? SecondName { get; set; }
        private string? Email { get; set; }
        private char FirstLetterOfName { get; set; }
        private bool _rightToLeft;
        private bool _isAdministrator;

        private async Task RightToLeftToggle()
        {
            var isRtl = await _clientPreferenceManager.ToggleLayoutDirection();
            _rightToLeft = isRtl;

            await OnRightToLeftToggle.InvokeAsync(isRtl);
        }

        public async Task ToggleDarkMode()
        {
            await OnDarkModeToggle.InvokeAsync();
        }

        protected override async Task OnInitializedAsync()
        {
            var currentUser = await _authenticationManager.CurrentUser();
            var role = currentUser.Claims.Where(x => x.Type == ApplicationClaimTypes.Role).Select(x=> x.Value).FirstOrDefault() ?? "";
            if(role.Contains("Administrator"))
            {
                _isAdministrator = true;
            }
            
            _rightToLeft = await _clientPreferenceManager.IsRtl();
            _interceptor.RegisterEvent();
            //_snackBar.Add($"Welcome {FirstName}", Severity.Success);
        }

        protected override async Task OnAfterRenderAsync(bool firstRender)
        {
            if (firstRender)
            {
                await LoadDataAsync();
            }
        }

        private async Task LoadDataAsync()
        {
            var state = await _stateProvider.GetAuthenticationStateAsync();
            var user = state.User;
            if (user.Identity?.IsAuthenticated == true)
            {
                if (string.IsNullOrEmpty(CurrentUserId))
                {
                    FirstName = user.Claims.FirstOrDefault(x => x.Type == "name")?.Value;
                    if (FirstName?.Length > 0)
                    {
                        FirstLetterOfName = FirstName[0];
                    }

                    SecondName = user.Username();
                    Email = user.UserEmail();
                    

                    var currentUser = await _authenticationManager.CurrentUser();
                    var role = currentUser.Claims.Where(x => x.Type == ApplicationClaimTypes.Role).Select(x=> x.Value).FirstOrDefault();
                    if (role == null || (!role.Contains("Administrator") && !role.Contains("Operations")))
                    {
                        // logout
                        _snackBar.Add("You are not authorized to use this application", Severity.Error);
                        CurrentUserId = string.Empty;
                        ImageDataUrl = string.Empty;
                        FirstName = string.Empty;
                        SecondName = string.Empty;
                        Email = string.Empty;
                        FirstLetterOfName = char.MinValue;
                        await _authenticationManager.Logout();
                    }
                    //var imageResponse = await _accountManager.GetProfilePictureAsync(CurrentUserId);
                    //if (imageResponse.Succeeded)
                    //{
                        //ImageDataUrl = imageResponse.Data;
                    //}
/*
                    var currentUserResult = await _userManager.GetAsync(CurrentUserId);
                    if (!currentUserResult.Succeeded || currentUserResult.Data == null)
                    {
                        _snackBar.Add(
                            "You are logged out because the user with your Token has been deleted.",
                            Severity.Error);
                        CurrentUserId = string.Empty;
                        ImageDataUrl = string.Empty;
                        FirstName = string.Empty;
                        SecondName = string.Empty;
                        Email = string.Empty;
                        FirstLetterOfName = char.MinValue;
                        await _authenticationManager.Logout();
                    }
                    
                    */
                }
            }
        }

        private MudMenu? _profileMenu;

        private void DrawerToggle()
        {
            _drawerOpen = !_drawerOpen;
        }
        
        private async Task NavigateToAccount()
        {
            await _profileMenu!.CloseMenuAsync();
            _navigationManager.NavigateTo("/account");
        }

        private async Task NavigateToManageUsers()
        {
            await _profileMenu!.CloseMenuAsync();
            _navigationManager.NavigateTo("/admin/manageUsers");
        }
        
        private async Task Logout()
        {
            await _profileMenu!.CloseMenuAsync();
            var parameters = new DialogParameters
            {
                {nameof(Confirmation.ConfirmationType), "Logout"},
                {nameof(Confirmation.TitleIcon), Icons.Material.Filled.Logout},
                {nameof(Confirmation.ContentText), "Are you sure you want to logout?"},
                {nameof(Confirmation.ButtonText), "Yes"},
                {nameof(Confirmation.Color), Color.Error}
            };

            var options = new DialogOptions { CloseButton = true, MaxWidth = MaxWidth.Small, FullWidth = true };

            var dialog = await _dialogService.ShowAsync<Confirmation>("Logout", parameters, options);
            var result = await dialog.Result;
            if (result is { Canceled: false } && (bool)result.Data!)
            {
                await _authenticationManager.Logout();
                _navigationManager.NavigateTo("/login");
            }
        }
    }