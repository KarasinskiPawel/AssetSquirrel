using AssetSquirrel.CoreBusiness;
using Microsoft.JSInterop;

namespace AssetSquirrelAuthorize.WebApp.Extensions
{
    public static class ResultToastExtensions
    {
        public static Task ShowToastAsync<T>(this Result<T> result, IJSRuntime jsRuntime, string successMessage, string failureFallback) =>
            jsRuntime.InvokeVoidAsync(
                result.Success ? "OperationSuccessful" : "OperationAborted",
                result.Message ?? (result.Success ? successMessage : failureFallback)).AsTask();
    }
}
