using Microsoft.Extensions.DependencyInjection;
using WaxRentalsWeb.Files;
using BananoDependencies = WaxRentals.Banano.Config.Dependencies;
using DataDependencies = WaxRentals.Data.Config.Dependencies;
using MonitoringDependencies = WaxRentals.Monitoring.Config.Dependencies;
using ServiceDependencies = WaxRentals.Service.Shared.Config.Dependencies;

namespace WaxRentalsWeb.Config
{
    public static class Dependencies
    {

        public static void AddDependencies(this IServiceCollection services)
        {
            DataDependencies.AddDependencies(services);
            BananoDependencies.AddDependencies(services);
            MonitoringDependencies.AddDependencies(services);
            ServiceDependencies.AddDependencies(services, "http://localhost:22022");

            services.AddSingleton<SiteMessageMonitor>();
        }

    }
}
