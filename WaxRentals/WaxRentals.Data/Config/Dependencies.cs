using System;
using System.Collections.Generic;
using System.Data.Entity.SqlServer;
using System.IO;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json.Linq;
using WaxRentals.Data.Context;
using WaxRentals.Data.Manager;
using static WaxRentals.Data.Config.Constants;

namespace WaxRentals.Data.Config
{
    public static class Dependencies
    {

        public static void AddDependencies(this IServiceCollection services)
        {
            var env = GetEnvironmentVariables();

            services.AddSingleton(provider =>
                JObject.Parse(
                    File.ReadAllText(env[EnvironmentVariables.DbFile])
                ).ToObject<WaxDb>()
            );

            services.AddTransient<WaxRentalsContext>();
            services.AddTransient<DataManager>();
            services.AddSingleton<IDataFactory, DataFactory>();

            // Allow the database scale to do rounding where necessary.
            SqlProviderServices.TruncateDecimalsToScale = false;
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
