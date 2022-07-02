using System;
using Microsoft.Extensions.DependencyInjection;
using WaxRentals.Service.Shared.Connectors;
using WaxRentalsWeb.Files;
using WaxRentalsWeb.Monitoring;
using ServiceDependencies = WaxRentals.Service.Shared.Config.Dependencies;

namespace WaxRentalsWeb.Config
{
    public static class Dependencies
    {

        public static void AddDependencies(this IServiceCollection services)
        {
            ServiceDependencies.AddDependencies(services, "http://localhost:22022");

            services.AddSingleton<SiteMessageMonitor>();

            services.AddSingleton<IAppStateMonitor>(provider =>
                new AppStateMonitor(
                    TimeSpan.FromSeconds(5),
                    provider.GetRequiredService<ITrackService>(),
                    provider.GetRequiredService<IAppService>()
                )
            );

            services.AddSingleton<IAppInsightsMonitor>(provider =>
                new AppInsightsMonitor(
                    TimeSpan.FromSeconds(5),
                    provider.GetRequiredService<ITrackService>(),
                    provider.GetRequiredService<IAppService>()
                )
            );
        }

    }
}
