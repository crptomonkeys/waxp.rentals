using WaxRentals.Banano.Transact;
using WaxRentals.Data.Manager;
using WaxRentals.Service.Caching;
using WaxRentals.Service.Http;
using WaxRentals.Waxp.Transact;
using static WaxRentals.Service.Config.Constants.Http;
using static WaxRentals.Service.Config.Constants.Locations;
using BananoDependencies = WaxRentals.Banano.Config.Dependencies;
using MonitoringDependencies = WaxRentals.Monitoring.Config.Dependencies;
using WaxDependencies = WaxRentals.Waxp.Config.Dependencies;

namespace WaxRentals.Service.Config
{
    public static class Dependencies
    {

        public static void AddDependencies(this IServiceCollection services)
        {
            // Protocols.

            BananoDependencies.AddDependencies(services);
            MonitoringDependencies.AddDependencies(services);
            WaxDependencies.AddDependencies(services);

            // Caches.

            services.AddSingleton(provider =>
                new BananoInfoCache(
                    provider.GetRequiredService<IDataFactory>(),
                    TimeSpan.FromMinutes(1),
                    provider.GetRequiredService<IBananoAccount>())
            );

            services.AddSingleton(provider =>
                new NftsCache(
                    provider.GetRequiredService<IDataFactory>(),
                    TimeSpan.FromMinutes(5)
                )
            );

            services.AddSingleton(provider =>
            {
                var factory = provider.GetRequiredService<IDataFactory>();
                return new PricesCache(
                    factory,
                    TimeSpan.FromMinutes(2),
                    BuildHttpClient(CoinPrices, factory)
                );
            });

            services.AddSingleton(provider =>
                new WaxInfoCache(
                    provider.GetRequiredService<IDataFactory>(),
                    TimeSpan.FromMinutes(2),
                    provider.GetRequiredService<IWaxAccounts>()
                )
            );

            services.AddSingleton<Cache>();
            services.AddSingleton<CostsCache>();
            services.AddSingleton<LimitsCache>();
        }

        #region " HttpClient "

        private static HttpClient BuildHttpClient(string endpoint, IDataFactory factory)
        {
            var handler = new MessageHandler(new HttpClientHandler(), factory);
            return new HttpClient(handler) { BaseAddress = new Uri(endpoint), Timeout = QuickTimeout };
        }

        #endregion

    }
}
