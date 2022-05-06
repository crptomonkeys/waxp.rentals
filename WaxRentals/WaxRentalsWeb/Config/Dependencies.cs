using Microsoft.Extensions.DependencyInjection;
using MonitoringDependencies = WaxRentals.Monitoring.Config.Dependencies;

namespace WaxRentalsWeb.Config
{
    public static class Dependencies
    {

        public static void AddDependencies(this IServiceCollection services)
        {
            MonitoringDependencies.AddDependencies(services);
        }

    }
}
