using Dnc.Services.RestClient.Options;
using Microsoft.Identity.Client;
using Microsoft.Identity.Web;
using System.Linq;

namespace Dnc.Services.RestClient.EntraId
{
    public class EntraIdAuthenticationService : IEntraIdAuthenticationService
    {
        private readonly IConfidentialClientApplication confidentialClientApplication;
        private readonly ITokenAcquisition tokenAcquisition;

        public EntraIdAuthenticationService(EntraIdOptions options, ITokenAcquisition tokenAcquisition)
        {
            this.tokenAcquisition = tokenAcquisition;

            var builder = ConfidentialClientApplicationBuilder.CreateWithApplicationOptions(new ConfidentialClientApplicationOptions
            {
                ClientId = options.ClientId,
                ClientSecret = options.ClientSecret,
                TenantId = options.TenantId
            });

            confidentialClientApplication = builder.Build();
        }

        // 1. Acquire token for the user (web-to-API scenario) : user token // Acquire token for interactive user
        public async Task<string> GetAccessTokenForUserAsync(IEnumerable<string> scopes)
        {
            try
            {
                return await tokenAcquisition.GetAccessTokenForUserAsync(scopes);
            }
            catch (MsalException ex) 
            { 
                throw new InvalidOperationException(ex.Message);
            }
        }

        // 2. Acquire token for the application (service-to-service scenario) : Credential flow
        public async Task<string> GetAccessTokenForAppAsync(string scope)
        {
            return await tokenAcquisition.GetAccessTokenForAppAsync(scope);
        }

        // 3. Acquire token on behalf of user (API-to-API scenario) : OBO flow
        public async Task<string> AcquireAccessTokenOnBehalfOf(IEnumerable<string> scopes, string assertion)
        {
            var builder = confidentialClientApplication.AcquireTokenOnBehalfOf(scopes, new UserAssertion(assertion));

            var authResult = await builder.ExecuteAsync();
            var accessToken = authResult.AccessToken;

            return accessToken;
        }
    }
}
