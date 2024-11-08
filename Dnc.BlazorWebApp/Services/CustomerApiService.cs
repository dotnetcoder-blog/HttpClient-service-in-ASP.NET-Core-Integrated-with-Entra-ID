using Dnc.Objects;
using Dnc.Services.RestClient;
using Dnc.Services.RestClient.Options;

namespace Dnc.BlazorWebApp.Services
{
    public interface ICustomerApiService
    {
        Task<IEnumerable<Customer>> GetCustomers();
        Task<IEnumerable<Customer>> GetCustomersWithOrders();
    }


    public class CustomerApiService : HttpRestClientBase, ICustomerApiService
    {
        public CustomerApiService(RestClientOptions restOptions, 
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

        public async Task<IEnumerable<Customer>> GetCustomers()
        {
            return await GetAsync<IEnumerable<Customer>>("customers/all");
        }
        public async Task<IEnumerable<Customer>> GetCustomersWithOrders()
        {
            return await GetAsync<IEnumerable<Customer>>("customers/orders/all");
        }
    }
}
