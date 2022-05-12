using Microsoft.Extensions.DependencyInjection;
using WaxRentals.Data.Manager;
using WaxRentals.Data.Context;
using static WaxRentals.Data.Config.Constants;
using Newtonsoft.Json.Linq;
using System.IO;

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
        }

    }
}
