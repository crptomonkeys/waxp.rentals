using System;
using System.Collections.Generic;
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

            var env = GetEnvironmentVariables();
            var seed = JObject.Parse(File.ReadAllText(env["BANANO_SEED_FILE"])).ToObject<BananoSeed>();
            var welcomeSeed = JObject.Parse(File.ReadAllText(env["BANANO_SEED_FILE_WELCOME"])).ToObject<BananoSeed>();

            services.AddSingleton(provider =>
                new StorageAccount(
                    seed,
                    provider.GetRequiredService<RpcClients>(),
                    provider.GetRequiredService<ILog>()
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
                    provider.GetRequiredService<ILog>()
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
