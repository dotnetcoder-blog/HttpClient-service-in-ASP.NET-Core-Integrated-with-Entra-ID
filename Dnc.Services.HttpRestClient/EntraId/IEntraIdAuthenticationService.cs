using Microsoft.Identity.Client;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.NetworkInformation;
using System.Text;
using System.Threading.Tasks;

namespace Dnc.Services.RestClient.EntraId
{
    public interface IEntraIdAuthenticationService
    {
        // Authorization code flow
        Task<string> GetAccessTokenForUserAsync(IEnumerable<string> scopes);
        // Client credentials flow
        Task<string> GetAccessTokenForAppAsync(string scope);
        // OBO flow
        Task<string> AcquireAccessTokenOnBehalfOf(IEnumerable<string> scopes, string assertion);
    }
}