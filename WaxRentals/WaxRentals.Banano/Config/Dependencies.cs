using System;
using System.IO;
using Microsoft.Extensions.DependencyInjection;
using Nano.Net;
using Newtonsoft.Json.Linq;
using WaxRentals.Banano.Monitoring;
using WaxRentals.Banano.Transact;
using WaxRentals.Data.Manager;
using WaxRentals.Monitoring;
using static WaxRentals.Banano.Config.Constants;

namespace WaxRentals.Banano.Config
{
    public static class Dependencies
    {

        public static void AddDependencies(this IServiceCollection services)
        {
            services.AddSingleton(provider =>
                JObject.Parse(
                    File.ReadAllText(Locations.Seed)
                ).ToObject<BananoSeed>()
            );
            
            services.AddSingleton(provider =>
                new RpcClients
                {
                    Node = new RpcClient(Locations.Node),
                    WorkServer = new RpcClient(Locations.WorkServer)
                }
            );

            services.AddSingleton(provider =>
                new BalanceMonitor(
                    TimeSpan.FromMinutes(2),
                    provider.GetRequiredService<ILog>(),
                    provider.GetRequiredService<ITransact>()
                )
            );

            services.AddSingleton<StorageAccount>();
            services.AddSingleton<ITransact, StorageAccount>(provider =>
                provider.GetRequiredService<StorageAccount>()
            );
            services.AddSingleton<IBananoAccountFactory, BananoAccountFactory>();
            services.AddSingleton<IGlobalMonitor, GlobalMonitor>();
        }

    }
}
