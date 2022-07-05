using System;
using System.Collections.Generic;
using System.IO;
using Eos.Cryptography;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json.Linq;
using WaxRentals.Data.Manager;
using WaxRentals.Waxp.History;
using WaxRentals.Waxp.Monitoring;
using WaxRentals.Waxp.Transact;
using static WaxRentals.Waxp.Config.Constants;

namespace WaxRentals.Waxp.Config
{
    public static class Dependencies
    {

        public static void AddDependencies(this IServiceCollection services)
        {
            var env = GetEnvironmentVariables();

            services.AddSingleton(provider =>
            {
                return new AccountNames
                {
                    Primary = env["WAX_PRIMARY"],
                    Transact = env["WAX_TRANSACT"].Split('+')
                };
            });

            services.AddSingleton(provider =>
                JObject.Parse(
                    File.ReadAllText(env["WAX_KEY_FILE"])
                ).ToObject<WaxKey>()
            );

            services.AddSingleton(provider =>
                new PrivateKey(provider.GetRequiredService<WaxKey>().Private)
            );

            services.AddSingleton(provider =>
                new EndpointMonitor(
                    TimeSpan.FromHours(1),
                    provider.GetRequiredService<IDataFactory>()
                )
            );

            services.AddSingleton<IClientFactory, ClientFactory>();
            services.AddSingleton<IWaxAccounts, WaxAccounts>();
            services.AddSingleton<IWaxHistoryChecker, WaxHistoryChecker>();
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
