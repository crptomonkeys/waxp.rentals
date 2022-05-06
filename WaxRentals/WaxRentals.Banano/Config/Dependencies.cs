using System.IO;
using Microsoft.Extensions.DependencyInjection;
using Nano.Net;
using Newtonsoft.Json.Linq;
using WaxRentals.Banano.Transact;
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

            services.AddSingleton<StorageAccount>();
            services.AddSingleton<GlobalMonitor>();
        }

    }
}
