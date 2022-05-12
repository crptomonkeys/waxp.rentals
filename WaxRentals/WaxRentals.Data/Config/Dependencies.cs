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
            services.AddSingleton(provider =>
                JObject.Parse(
                    File.ReadAllText(Locations.Db)
                ).ToObject<WaxDb>()
            );

            services.AddTransient<WaxRentalsContext>();
            services.AddTransient<IInsert, DataManager>();
            services.AddTransient<IProcess, DataManager>();
            services.AddTransient<ILog, DataManager>();
        }

    }
}
