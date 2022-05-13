using System;
using Microsoft.Extensions.DependencyInjection;
using WaxRentals.Data.Manager;
using WaxRentals.Monitoring.Prices;
using static WaxRentals.Monitoring.Config.Constants;

namespace WaxRentals.Monitoring.Config
{
    public static class Dependencies
    {

        public static void AddDependencies(this IServiceCollection services)
        {
            services.AddSingleton<IPriceMonitor>(provider =>
                new PriceMonitor(
                    TimeSpan.FromMinutes(2),
                    provider.GetRequiredService<ILog>(),
                    $"https://api.coingecko.com/api/v3/simple/price?vs_currencies=usd&include_24hr_change=true&ids={Coins.Banano},{Coins.Wax}"
                )
            );
        }

    }
}
