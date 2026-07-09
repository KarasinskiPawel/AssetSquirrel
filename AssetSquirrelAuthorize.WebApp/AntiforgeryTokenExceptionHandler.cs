using Microsoft.AspNetCore.Antiforgery;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Http;

namespace AssetSquirrelAuthorize.WebApp;

/// <summary>
/// A page left open across a Data Protection key rotation (e.g. an IIS app-pool recycle) carries a stale
/// antiforgery token. Rather than surfacing that as a 500/error page, send the user back to Login.
/// </summary>
public sealed class AntiforgeryTokenExceptionHandler : IExceptionHandler
{
    public ValueTask<bool> TryHandleAsync(HttpContext httpContext, Exception exception, CancellationToken cancellationToken)
    {
        if (!IsAntiforgeryValidationFailure(exception))
        {
            return ValueTask.FromResult(false);
        }

        httpContext.Response.Redirect("/Account/Login");
        return ValueTask.FromResult(true);
    }

    internal static bool IsAntiforgeryValidationFailure(Exception exception) =>
        exception is AntiforgeryValidationException
        || exception is BadHttpRequestException { InnerException: AntiforgeryValidationException };
}
