using Microsoft.Extensions.DependencyInjection;
using WaxRentals.Processing.Processors;
using WaxRentals.Processing.Tracking;

namespace WaxRentals.Processing.Config
{
    internal static class Dependencies
    {

        public static void AddDependencies(this IServiceCollection services)
        {
            // Processors.

            services.AddSingleton<RentalOpenProcessor>();
            services.AddSingleton<RentalStakeProcessor>();
            services.AddSingleton<RentalSweepProcessor>();
            services.AddSingleton<RentalClosingProcessor>();

            services.AddSingleton<PurchaseProcessor>();

            //services.AddSingleton<WorkProcessor>();

            services.AddSingleton<DayChangeProcessor>();

            services.AddSingleton<TrackBananoProcessor>();
            services.AddSingleton<TrackWaxProcessor>();

            services.AddSingleton<WelcomePackageOpenProcessor>();
            services.AddSingleton<WelcomePackageFundingProcessor>();
            services.AddSingleton<WelcomePackageNftProcessor>();
            services.AddSingleton<WelcomePackageRentalProcessor>();
            services.AddSingleton<WelcomePackageSweepProcessor>();


            // Other.

            services.AddSingleton<ITracker, Tracker>();
        }

    }
}
