using Dnc.Services.RestClient.EntraId;
using Dnc.Services.RestClient.Options;
using Microsoft.Extensions.DependencyInjection;

namespace Dnc.Services.RestClient.Extensions
{
    public static class EntraIdExtensions
    {
        public static IServiceCollection AddEntraIdAuthenticationService<TInterface, TImplementation>(
            this IServiceCollection services, 
            Action<EntraIdOptions> configureOptions)
            where TImplementation : TInterface, IEntraIdAuthenticationService
            where TInterface : class
        {
            services.AddScoped<TInterface>(provider =>
            {
                var options = new EntraIdOptions();
                configureOptions?.Invoke(options);
                return ActivatorUtilities.CreateInstance<TImplementation>(provider, options);
            });

            return services;
        }

    }
}
