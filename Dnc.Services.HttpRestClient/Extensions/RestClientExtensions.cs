using Dnc.Services.RestClient.Options;
using Microsoft.Extensions.DependencyInjection;

namespace Dnc.Services.RestClient.Extensions
{
    public static class RestClientExtensions
    {
        public static IServiceCollection AddRestClientService<TInterface, TImplementation>(
            this IServiceCollection services, 
            Action<RestClientOptions> configureOptions)
            where TImplementation : HttpRestClientBase, TInterface
            where TInterface : class
        {
            services.AddScoped<TInterface>(provider =>
            {
                var options = new RestClientOptions();
                configureOptions?.Invoke(options);
                return ActivatorUtilities.CreateInstance<TImplementation>(provider, options);
            });

            return services;
        }
    }
}
