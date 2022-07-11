using System;
using System.Collections.Generic;
using Microsoft.Extensions.DependencyInjection;
using WaxRentals.Service.Shared.Connectors;
using WaxRentalsWeb.Files;
using WaxRentalsWeb.Monitoring;
using WaxRentalsWeb.Net;
using static WaxRentalsWeb.Config.Constants;
using ServiceDependencies = WaxRentals.Service.Shared.Config.Dependencies;

namespace WaxRentalsWeb.Config
{
    public static class Dependencies
    {

        public static void AddDependencies(this IServiceCollection services)
        {
            var env = GetEnvironmentVariables();

            ServiceDependencies.AddLogDependencies(services, env[EnvironmentVariables.Service]);

            services.AddSingleton(provider => new ApiContext(env[EnvironmentVariables.Api]));
            services.AddSingleton<ApiProxy>();

            services.AddSingleton<SiteMessageMonitor>();

            services.AddSingleton<IAppStateMonitor>(provider =>
                new AppStateMonitor(
                    TimeSpan.FromSeconds(5),
                    provider.GetRequiredService<ITrackService>(),
                    provider.GetRequiredService<ApiProxy>()
                )
            );

            services.AddSingleton<IAppInsightsMonitor>(provider =>
                new AppInsightsMonitor(
                    TimeSpan.FromSeconds(5),
                    provider.GetRequiredService<ITrackService>(),
                    provider.GetRequiredService<ApiProxy>()
                )
            );
        }

        private static IDictionary<string, string> GetEnvironmentVariables()
        {
            var env = Environment.GetEnvironmentVariables();
            var dic = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
            foreach (string key in env.Keys)
            {
                dic.Add(key, (string)env[key]);
            }
            return dic;
        }

    }
}
