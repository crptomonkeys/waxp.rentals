using System;
using Microsoft.Extensions.DependencyInjection;
using WaxRentals.Processing.Processors;
using BananoDependencies = WaxRentals.Banano.Config.Dependencies;
using DataDependencies = WaxRentals.Data.Config.Dependencies;
using MonitoringDependencies = WaxRentals.Monitoring.Config.Dependencies;
using WaxDependencies = WaxRentals.Waxp.Config.Dependencies;

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

            //var work = provider.GetRequiredService<WorkProcessor>();
            //work.Start(TimeSpan.FromSeconds(5)); // Be very responsive to work needs.

            var dayChange = provider.GetRequiredService<DayChangeProcessor>();
            dayChange.Start(TimeSpan.FromSeconds(5)); // Be very responsive to the day changing.

            Console.WriteLine("Processors running.  Hit any key to exit.");
            Console.ReadKey();
        }

        private static IServiceProvider BuildServiceProvider()
        {
            var services = new ServiceCollection();

            DataDependencies.AddDependencies(services);
            BananoDependencies.AddDependencies(services);
            WaxDependencies.AddDependencies(services);
            MonitoringDependencies.AddDependencies(services);

            services.AddSingleton<RentalProcessor>();
            services.AddSingleton<PurchaseProcessor>();
            services.AddSingleton<RentalClosingProcessor>();
            //services.AddSingleton<WorkProcessor>();
            services.AddSingleton<DayChangeProcessor>();

            return services.BuildServiceProvider();
        }
    }
}
