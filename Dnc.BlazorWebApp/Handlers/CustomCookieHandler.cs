using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.OpenIdConnect;
using Microsoft.Identity.Web;

namespace Dnc.BlazorWebApp.Handlers
{
    public class CustomCookieHandler : CookieAuthenticationEvents
    {
        public override async Task ValidatePrincipal(CookieValidatePrincipalContext context)
        {
            // Custom validation logic
            var userPrincipal = context.Principal;
            // Example condition to reject the cookie
            if (!userPrincipal.Identity.IsAuthenticated)
            {
                context.RejectPrincipal();
                await context.HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            }

            try
            {
                var tokenAcquisition = context.HttpContext.RequestServices.GetRequiredService<ITokenAcquisition>();

                // Ensure the logged in user exist.
                var token = await tokenAcquisition.GetAccessTokenForUserAsync(
                    scopes: ["profile"],
                    user: context.Principal);
            }
            catch (MicrosoftIdentityWebChallengeUserException)
            {
                //context.RejectPrincipal();
                // Redirect to login page to handle consent or reauthentication
                var authProperties = new AuthenticationProperties
                {
                    RedirectUri = context.HttpContext.Request.Path, // Redirect back to the current page
                };

                await context.HttpContext.ChallengeAsync(OpenIdConnectDefaults.AuthenticationScheme, authProperties);
            }

        }
    }
}
