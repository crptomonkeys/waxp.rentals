using System;
using System.IO;
using Eos.Cryptography;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json.Linq;
using WaxRentals.Data.Manager;
using WaxRentals.Monitoring;
using WaxRentals.Waxp.Monitoring;
using WaxRentals.Waxp.Transact;
using static WaxRentals.Waxp.Config.Constants;

namespace WaxRentals.Waxp.Config
{
    public static class Dependencies
    {

        public static void AddDependencies(this IServiceCollection services)
        {
            services.AddSingleton(provider =>
                JObject.Parse(
                    File.ReadAllText(Locations.Key)
                ).ToObject<WaxKey>()
            );

            services.AddSingleton(provider =>
                new PrivateKey(provider.GetRequiredService<WaxKey>().Private)
            );

            services.AddSingleton(provider =>
                new EndpointMonitor(
                    TimeSpan.FromHours(1),
                    provider.GetRequiredService<ILog>()
                )
            );

            services.AddSingleton<ClientFactory>();

            services.AddSingleton<ITransact>(provider =>
                new WrappedAccount(
                    Protocol.Account,
                    provider.GetRequiredService<PrivateKey>(),
                    provider.GetRequiredService<ClientFactory>()
                )
            );

            services.AddSingleton(provider =>
                new HistoryMonitor(
                    TimeSpan.FromSeconds(30),
                    provider.GetRequiredService<ILog>(),
                    Protocol.Account,
                    provider.GetRequiredService<ClientFactory>(),
                    provider.GetRequiredService<ITrackWax>()
                )
            );

            services.AddSingleton<IGlobalMonitor, GlobalMonitor>();
        }

    }
}
