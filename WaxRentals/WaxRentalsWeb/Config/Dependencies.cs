using Microsoft.Extensions.DependencyInjection;
using WaxRentalsWeb.Data;
using WaxRentalsWeb.Files;
using BananoDependencies = WaxRentals.Banano.Config.Dependencies;
using DataDependencies = WaxRentals.Data.Config.Dependencies;
using MonitoringDependencies = WaxRentals.Monitoring.Config.Dependencies;
using ServiceDependencies = WaxRentals.Service.Shared.Config.Dependencies;
using WaxDependencies = WaxRentals.Waxp.Config.Dependencies;

namespace WaxRentalsWeb.Config
{
    public static class Dependencies
    {

        public static void AddDependencies(this IServiceCollection services)
        {
            DataDependencies.AddDependencies(services);
            MonitoringDependencies.AddDependencies(services);
            BananoDependencies.AddDependencies(services);
            WaxDependencies.AddDependencies(services);
            ServiceDependencies.AddDependencies(services, "http://localhost:22022");

            services.AddSingleton<SiteMessageMonitor>();

            services.AddSingleton<DataCache>();
            services.AddSingleton<IDataCache>(provider => provider.GetRequiredService<DataCache>());
        }

    }
}
