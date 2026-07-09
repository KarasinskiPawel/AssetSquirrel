using AssetSquirrelAuthorize.WebApp;
using Microsoft.AspNetCore.Antiforgery;
using Microsoft.AspNetCore.Http;

namespace AssetSquirrelAuthorize.WebApp.Tests;

public class AntiforgeryTokenExceptionHandlerTests
{
    private readonly AntiforgeryTokenExceptionHandler _sut = new();

    [Fact]
    public async Task TryHandleAsync_AntiforgeryValidationException_RedirectsToLoginAndHandles()
    {
        var context = new DefaultHttpContext();

        var handled = await _sut.TryHandleAsync(context, new AntiforgeryValidationException("boom"), CancellationToken.None);

        Assert.True(handled);
        Assert.Equal(302, context.Response.StatusCode);
        Assert.Equal("/Account/Login", context.Response.Headers.Location);
    }

    [Fact]
    public async Task TryHandleAsync_BadHttpRequestExceptionWrappingAntiforgeryFailure_RedirectsToLoginAndHandles()
    {
        var context = new DefaultHttpContext();
        var wrapped = new BadHttpRequestException("Invalid anti-forgery token found when reading parameter \"string returnUrl\" from the request body as form.",
            innerException: new AntiforgeryValidationException("boom"));

        var handled = await _sut.TryHandleAsync(context, wrapped, CancellationToken.None);

        Assert.True(handled);
        Assert.Equal(302, context.Response.StatusCode);
        Assert.Equal("/Account/Login", context.Response.Headers.Location);
    }

    [Fact]
    public async Task TryHandleAsync_UnrelatedException_DoesNotHandleAndLeavesResponseUntouched()
    {
        var context = new DefaultHttpContext();

        var handled = await _sut.TryHandleAsync(context, new InvalidOperationException("unrelated"), CancellationToken.None);

        Assert.False(handled);
        Assert.Equal(200, context.Response.StatusCode);
        Assert.False(context.Response.Headers.ContainsKey("Location"));
    }
}
