using HsaLedger.Shared.Wrapper;
using MudBlazor;

namespace HsaLedger.Client.Common;

internal static class AlertHelper
{
    internal const string DeleteFallbackErrorMessage = "An unknown error occurred while attempting to delete data";
    internal const string UpdateFallbackErrorMessage = "An unknown error occurred while attempting to update data";
    internal const string AddFallbackErrorMessage = "An unknown error occurred while attempting to add data";
    internal const string GetFallbackErrorMessage = "An unknown error occurred while attempting to get data";
    internal static async Task DataAccessAlertHandler(ISnackbar snackbar,
        IResult result,
        string successMessage,
        string fallbackErrorMessage,
        Func<Task>? onSuccess = null,
        Func<Task>? onFail = null)
    {
        if (result.Succeeded)
        {
            snackbar.Add(successMessage, Severity.Success);
            if (onSuccess != null)
            {
                await onSuccess();
            }
        }
        else
        {
            if (result.Messages == null)
            {
                snackbar.Add(fallbackErrorMessage, Severity.Error);
            }
            else
            {
                foreach (var message in result.Messages)
                {
                    snackbar.Add(message, Severity.Error);
                }    
            }

            if (onFail != null)
            {
                await onFail();
            }
        }
    }
}