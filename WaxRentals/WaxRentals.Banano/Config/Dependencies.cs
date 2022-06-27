using System.IO;
using Microsoft.Extensions.DependencyInjection;
using Nano.Net;
using Newtonsoft.Json.Linq;
using WaxRentals.Banano.Transact;
using WaxRentals.Data.Manager;
using static WaxRentals.Banano.Config.Constants;

namespace WaxRentals.Banano.Config
{
    public static class Dependencies
    {

        public static void AddDependencies(this IServiceCollection services)
        {
            services.AddSingleton(provider =>
                new RpcClients
                {
                    Node = new RpcClient(Locations.Node),
                    WorkServer = new RpcClient(Locations.WorkServer)
                }
            );

            var seed = JObject.Parse(File.ReadAllText(Locations.Seed)).ToObject<BananoSeed>();
            var welcomeSeed = JObject.Parse(File.ReadAllText(Locations.WelcomeSeed)).ToObject<BananoSeed>();

            services.AddSingleton(provider =>
                new StorageAccount(
                    seed,
                    provider.GetRequiredService<RpcClients>(),
                    provider.GetRequiredService<IDataFactory>()
                )
            );

            services.AddSingleton<IBananoAccount, StorageAccount>(provider =>
                provider.GetRequiredService<StorageAccount>()
            );

            services.AddSingleton<IBananoAccountFactory>(provider =>
                new BananoAccountFactory(
                    seed,
                    welcomeSeed,
                    provider.GetRequiredService<RpcClients>(),
                    provider.GetRequiredService<IDataFactory>()
                )
            );
        }

    }
}
