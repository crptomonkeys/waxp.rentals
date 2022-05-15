using System;
using Microsoft.Extensions.DependencyInjection;
using WaxDependencies = WaxRentals.Waxp.Config.Dependencies;
using BananoDependencies = WaxRentals.Banano.Config.Dependencies;
using DataDependencies = WaxRentals.Data.Config.Dependencies;
using WaxRentals.Processing.Processors;

namespace WaxRentals.Processing
{
    class Program
    {
        static void Main(string[] args)
        {
            var provider = BuildServiceProvider();

            var credit = provider.GetRequiredService<RentalProcessor>();
            credit.Start(TimeSpan.FromSeconds(10)); // Be responsive on credits.
            
            var payment = provider.GetRequiredService<PurchaseProcessor>();
            payment.Start(TimeSpan.FromSeconds(10)); // Be responsive on credits.

            var closing = provider.GetRequiredService<RentalClosingProcessor>();
            closing.Start(TimeSpan.FromMinutes(5)); // Be generous on debits.
            
            Console.WriteLine("Processors running.  Hit any key to exit.");
            Console.ReadKey();
        }

        private static IServiceProvider BuildServiceProvider()
        {
            var services = new ServiceCollection();
            DataDependencies.AddDependencies(services);
            BananoDependencies.AddDependencies(services);
            WaxDependencies.AddDependencies(services);
            services.AddSingleton<RentalProcessor>();
            services.AddSingleton<PurchaseProcessor>();
            services.AddSingleton<RentalClosingProcessor>();
            return services.BuildServiceProvider();
        }
    }
}
