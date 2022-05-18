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
            services.AddTransient<DataManager>();
            services.AddSingleton<IDataFactory, DataFactory>();
        }

    }
}
