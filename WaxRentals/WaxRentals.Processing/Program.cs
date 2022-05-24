using System;
using System.Linq;
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
            Console.WriteLine("Starting up; please wait.");
            var provider = BuildServiceProvider();
            var processors = new IProcessor[]
            {
                // Be responsive on credits.
                provider.BuildProcessor<RentalOpenProcessor>(TimeSpan.FromSeconds(10)),
                provider.BuildProcessor<RentalStakeProcessor>(TimeSpan.FromSeconds(10)),
                provider.BuildProcessor<PurchaseProcessor>(TimeSpan.FromSeconds(10)),

                // Be responsive on credits but don't annoy the node operators.
                provider.BuildProcessor<TrackWaxProcessor>(TimeSpan.FromSeconds(30)),

                // Be generous on debits.
                provider.BuildProcessor<RentalClosingProcessor>(TimeSpan.FromMinutes(5)),

                // Be very responsive to work needs.
                //provider.BuildProcessor<WorkProcessor>(TimeSpan.FromSeconds(5)),

                // Be very responsive to the day changing.
                provider.BuildProcessor<DayChangeProcessor>(TimeSpan.FromSeconds(5)),

                // Don't have to be that quick on sweeping transactions.
                provider.BuildProcessor<RentalSweepProcessor>(TimeSpan.FromMinutes(1)),
                
                // This is just for tracking purposes; don't have to check that often.
                provider.BuildProcessor<TrackBananoProcessor>(TimeSpan.FromMinutes(1)),
            };
            Console.WriteLine("Processors running.  Press any key to shut down.");
            
            Console.Read(); // Read instead of ReadKey because ReadKey isn't available when running in a container.
            Console.WriteLine("Shutting down; please wait.");
            var complete = processors.Select(processor => processor.Stop());
            while (complete.Any(mres => !mres.IsSet))
            {
                var first = complete.FirstOrDefault(mres => !mres.IsSet);
                if (first != null)
                {
                    first.Wait();
                }
            }
            Console.WriteLine("Shutdown complete.");
        }

        private static IServiceProvider BuildServiceProvider()
        {
            var services = new ServiceCollection();

            DataDependencies.AddDependencies(services);
            BananoDependencies.AddDependencies(services);
            WaxDependencies.AddDependencies(services);
            MonitoringDependencies.AddDependencies(services);

            services.AddSingleton<RentalOpenProcessor>();
            services.AddSingleton<RentalStakeProcessor>();
            services.AddSingleton<RentalSweepProcessor>();
            services.AddSingleton<PurchaseProcessor>();
            services.AddSingleton<RentalClosingProcessor>();
            //services.AddSingleton<WorkProcessor>();
            services.AddSingleton<DayChangeProcessor>();
            services.AddSingleton<TrackBananoProcessor>();
            services.AddSingleton<TrackWaxProcessor>();

            return services.BuildServiceProvider();
        }
    }

    internal static class IServiceProviderExtensions
    {
        public static T BuildProcessor<T>(this IServiceProvider @this, TimeSpan interval)
            where T : IProcessor
        {
            var processor = @this.GetRequiredService<T>();
            processor.Start(interval);
            return processor;
        }
    }
}
