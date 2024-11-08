using Dnc.Services.RestClient.EntraId;
using Dnc.Services.RestClient.Options;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace Dnc.Services.RestClient
{
    public abstract class HttpRestClientBase
    {
        protected HttpClient HttpClient;
        protected IHttpContextAccessor HttpContextAccessor;
        protected IEntraIdAuthenticationService AzureAdAuthenticationService;
        protected string UserScope;
        protected string AppScope;
        protected string Audience;

        protected void ConfigureRestClient(Action<RestClientOptions> configureOptions)
        {
            var options = new RestClientOptions();
            configureOptions(options);

            HttpClient = options.HttpClient;
            UserScope = options.UserScope;
            AppScope = options.AppScope;
            Audience = options.Audience;

            HttpContextAccessor = options.ServiceProvider.GetRequiredService<IHttpContextAccessor>();
            AzureAdAuthenticationService = options.ServiceProvider.GetRequiredService<IEntraIdAuthenticationService>();
        }

        public async Task<TResult> GetAsync<TResult>(string path) where TResult : class
        {
            var request = await CreateRequestMessage(HttpMethod.Get, path);
            var response = await HttpClient.SendAsync(request);

            var content = await response.Content.ReadAsStringAsync();

            if (response.IsSuccessStatusCode)
            {
                var options = new JsonSerializerOptions
                {
                    PropertyNamingPolicy = JsonNamingPolicy.CamelCase, 
                    PropertyNameCaseInsensitive = true,               
                    DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull, 
                };
                return JsonSerializer.Deserialize<TResult>(content, options);
            }
            else
            {
                throw new HttpRequestException($"Request failed with status {response.StatusCode}: {content}");
            }
        }
        public async Task<T> PostAsync<T>(string uri, object payload = null, 
            Dictionary<string, string> headers = null)
        {
            var request = await CreateRequestMessage(HttpMethod.Post, uri);

            request = HandleContent(request, payload, headers);

            var response = await HttpClient.SendAsync(request);

            var content = await response.Content.ReadAsStringAsync();

            if (response.IsSuccessStatusCode)
            {
                return JsonSerializer.Deserialize<T>(content);
            }
            else
            {
                throw new HttpRequestException($"Request failed with status {response.StatusCode}: {content}");
            }
        }
        public async Task<T> PutAsync<T>(string uri, object payload = null, 
            Dictionary<string, string> headers = null)
        {
            var request = await CreateRequestMessage(HttpMethod.Put, uri);
            request = HandleContent(request, payload, headers);

            var response = await HttpClient.SendAsync(request);
            var content = await response.Content.ReadAsStringAsync();

            if (response.IsSuccessStatusCode)
            {
                return JsonSerializer.Deserialize<T>(content);
            }
            else
            {
                throw new HttpRequestException($"Request failed with status {response.StatusCode}: {content}");
            }
        }

        private async Task<HttpRequestMessage> CreateRequestMessage(HttpMethod method, string requestUri)
        {
            var request = new HttpRequestMessage(method, requestUri);
            var accessToken = await AcquireAccessTokenBasedOnContext();
            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);
            return request;
        }
        private static HttpRequestMessage HandleContent(HttpRequestMessage request, object payload = null, 
            Dictionary<string, string> headers = null)
        {
            // Handling different content types
            if (payload is MultipartFormDataContent multipartData)
            {
                request.Content = multipartData;
            }
            else if (payload != null && payload is string || payload.GetType() == typeof(string))
            {
                var json = payload.GetType() == typeof(string) ? payload as string : JsonSerializer.Serialize(payload);
                request.Content = new StringContent(json, Encoding.UTF8, "application/json");
            }
            else if (payload != null)
            {
                request.Content = new StringContent(payload.ToString(), Encoding.UTF8, "application/x-www-form-urlencoded");
            }

            if (headers != null)
            {
                foreach (var header in headers.Where(v => !string.IsNullOrWhiteSpace(v.Value)))
                {
                    request.Headers.Remove(header.Key);
                    request.Headers.TryAddWithoutValidation(header.Key, header.Value);
                }
            }

            return request;
        }
        private async Task<string> AcquireAccessTokenBasedOnContext()
        {
            var user = HttpContextAccessor.HttpContext?.User;
            string accessToken;

            if (user == null || !user.Identity.IsAuthenticated)
            {
                // No authenticated user, service-to-service scenario
                accessToken = await AzureAdAuthenticationService.GetAccessTokenForAppAsync($"api://{Audience}/{AppScope}");
            }
            else
            {
                // Authenticated user or API-to-API scenario
                var inboundToken = GetInboundTokenFromRequest();
                accessToken = string.IsNullOrEmpty(inboundToken)
                    ? await AzureAdAuthenticationService.GetAccessTokenForUserAsync([$"api://{Audience}/{UserScope}"])
                    : await AzureAdAuthenticationService.AcquireAccessTokenOnBehalfOf([$"api://{Audience}/{UserScope}"], inboundToken);
            }

            return accessToken;
        }
        private string GetInboundTokenFromRequest()
        {
            var inboundToken = HttpContextAccessor.HttpContext?.Request?.Headers?.Authorization.FirstOrDefault();
            if (inboundToken != null && inboundToken.StartsWith("Bearer "))
            {
                return inboundToken[7..];
            }
            return null;
        }
    }
}
