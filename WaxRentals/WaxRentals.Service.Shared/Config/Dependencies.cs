using System;
using System.Linq;
using Microsoft.Extensions.DependencyInjection;
using WaxRentals.Service.Shared.Connectors;

namespace WaxRentals.Service.Shared.Config
{
    public static class Dependencies
    {

        public static void AddDependencies(this IServiceCollection services, string baseUrl)
        {
            AddLogDependencies(services, baseUrl);

            services.AddSingleton<IAppService>(provider =>
                new AppService(
                    BuildUrl(baseUrl, "App"),
                    provider.GetRequiredService<ITrackService>()
                )
            );

            services.AddSingleton<IBananoService>(provider =>
                new BananoService(
                    BuildUrl(baseUrl, "Banano"),
                    provider.GetRequiredService<ITrackService>()
                )
            );

            services.AddSingleton<IPurchaseService>(provider =>
                new PurchaseService(
                    BuildUrl(baseUrl, "Purchase"),
                    provider.GetRequiredService<ITrackService>()
                )
            );

            services.AddSingleton<IRentalService>(provider =>
                new RentalService(
                    BuildUrl(baseUrl, "Rental"),
                    provider.GetRequiredService<ITrackService>()
                )
            );

            services.AddSingleton<IWaxService>(provider =>
                new WaxService(
                    BuildUrl(baseUrl, "Wax"),
                    provider.GetRequiredService<ITrackService>()
                )
            );

            services.AddSingleton<IWelcomePackageService>(provider =>
                new WelcomePackageService(
                    BuildUrl(baseUrl, "WelcomePackage"),
                    provider.GetRequiredService<ITrackService>()
                )
            );
        }

        public static void AddLogDependencies(this IServiceCollection services, string baseUrl)
        {
            // Don't double up if both methods are called.
            if (!services.Any(service => service.ServiceType == typeof(ITrackService)))
            {
                services.AddSingleton<ITrackService>(provider =>
                    new TrackService(
                        BuildUrl(baseUrl, "Track")
                    )
                );
            }
        }

        private static Uri BuildUrl(string baseUrl, string name)
        {
            var service = new Uri(baseUrl);
            return new Uri(service, $"/{name}/");
        }

    }
}
