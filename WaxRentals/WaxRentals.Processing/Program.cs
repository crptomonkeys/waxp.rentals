using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using Microsoft.Extensions.DependencyInjection;
using WaxRentals.Processing.Processors;
using ProcessingDependencies = WaxRentals.Processing.Config.Dependencies;
using ServiceDependencies = WaxRentals.Service.Shared.Config.Dependencies;

namespace WaxRentals.Processing
{
    class Program
    {
        static void Main()
        {
            Console.WriteLine("Starting up; please wait.");
            var provider = BuildServiceProvider();
            var processors = new IProcessor[]
            {
                // Be responsive on credits.
                provider.BuildProcessor<RentalOpenProcessor>(TimeSpan.FromSeconds(10)),
                provider.BuildProcessor<RentalStakeProcessor>(TimeSpan.FromSeconds(10)),
                provider.BuildProcessor<PurchaseProcessor>(TimeSpan.FromSeconds(10)),
                provider.BuildProcessor<WelcomePackageOpenProcessor>(TimeSpan.FromSeconds(10)),
                provider.BuildProcessor<WelcomePackageFundingProcessor>(TimeSpan.FromSeconds(10)),
                provider.BuildProcessor<WelcomePackageNftProcessor>(TimeSpan.FromSeconds(10)),
                provider.BuildProcessor<WelcomePackageRentalProcessor>(TimeSpan.FromSeconds(10)),

                // Be responsive on credits but don't annoy the node operators.
                provider.BuildProcessor<TrackWaxProcessor>(TimeSpan.FromSeconds(30)),

                // Be generous on debits.
                provider.BuildProcessor<RentalClosingProcessor>(TimeSpan.FromMinutes(5)),

                // Be very responsive to the day changing.
                provider.BuildProcessor<DayChangeProcessor>(TimeSpan.FromSeconds(5)),

                // Don't have to be that quick on sweeping transactions.
                provider.BuildProcessor<RentalSweepProcessor>(TimeSpan.FromMinutes(1)),
                provider.BuildProcessor<WelcomePackageSweepProcessor>(TimeSpan.FromMinutes(1)),
                
                // This is just for tracking purposes; don't have to check that often.
                provider.BuildProcessor<TrackBananoProcessor>(TimeSpan.FromMinutes(1)),
            };

            var stop = new ManualResetEventSlim();
            AppDomain.CurrentDomain.ProcessExit += (_, _) => stop.Set();
            Console.WriteLine("Processors running.  Awaiting SIGTERM.");

            stop.Wait();
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
            var env = GetEnvironmentVariables();
            var services = new ServiceCollection();

            ServiceDependencies.AddDependencies(services, env["SERVICE"]);
            ProcessingDependencies.AddDependencies(services);

            return services.BuildServiceProvider();
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
