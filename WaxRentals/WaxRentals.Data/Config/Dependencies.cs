using Microsoft.Extensions.DependencyInjection;
using WaxRentals.Data.Access;
using WaxRentals.Data.Context;

namespace WaxRentals.Data.Config
{
    public static class Dependencies
    {

        public static void AddDependencies(this IServiceCollection services)
        {
            services.AddTransient<WaxRentalsContext>();
            services.AddTransient<IInsert, DataManager>();
            services.AddTransient<IProcess, DataManager>();
        }

    }
}
