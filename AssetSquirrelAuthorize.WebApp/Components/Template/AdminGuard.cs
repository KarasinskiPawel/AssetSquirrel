using Microsoft.AspNetCore.Components.Authorization;

namespace AssetSquirrelAuthorize.WebApp.Components.Template
{
    public static class AdminGuard
    {
        public static async Task<bool> IsAdminAsync(AuthenticationStateProvider authenticationStateProvider)
        {
            var authState = await authenticationStateProvider.GetAuthenticationStateAsync();
            return authState.User.IsInRole("Admin");
        }
    }
}
