using System;
using Microsoft.Extensions.DependencyInjection;
using WaxRentals.Data.Manager;
using WaxRentals.Service.Shared.Connectors;

namespace WaxRentals.Service.Shared.Config
{
    public static class Dependencies
    {

        public static void AddDependencies(this IServiceCollection services, string baseUrl)
        {
            services.AddSingleton<IAppService>(provider =>
                new AppService(
                    BuildUrl(baseUrl, "App"),
                    provider.GetRequiredService<IDataFactory>().Log
                )
            );

            services.AddSingleton<IRentalService>(provider =>
                new RentalService(
                    BuildUrl(baseUrl, "Rental"),
                    provider.GetRequiredService<IDataFactory>().Log
                )
            );

            services.AddSingleton<IWelcomePackageService>(provider =>
                new WelcomePackageService(
                    BuildUrl(baseUrl, "WelcomePackage"),
                    provider.GetRequiredService<IDataFactory>().Log
                )
            );
        }

        private static Uri BuildUrl(string baseUrl, string name)
        {
            var service = new Uri(baseUrl);
            return new Uri(service, $"/{name}/");
        }

    }
}
