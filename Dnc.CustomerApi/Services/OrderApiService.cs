using Dnc.Objects;
using Dnc.Services.RestClient;
using Dnc.Services.RestClient.Options;

namespace Dnc.CustomerApi.Services
{
    public interface IOrderApiService
    {
        Task<IEnumerable<Order>> GetOrders();
    }
    public class OrderApiService : HttpRestClientBase, IOrderApiService
    {
        public OrderApiService(RestClientOptions restOptions,
            IHttpClientFactory httpClientFactory,
            IServiceProvider provider)
        {
            var httpClient = httpClientFactory.CreateClient();
            httpClient.BaseAddress = new Uri(restOptions.BaseAddress);

            ConfigureRestClient(options =>
            {
                options.HttpClient = httpClient;
                options.ServiceProvider = provider;
                options.UserScope = restOptions.UserScope;
                options.AppScope = restOptions.AppScope;
                options.Audience = restOptions.Audience;
            });
        }
        public async Task<IEnumerable<Order>> GetOrders()
        {
            return await GetAsync<IEnumerable<Order>>("orders/all");
        }
    }
}
