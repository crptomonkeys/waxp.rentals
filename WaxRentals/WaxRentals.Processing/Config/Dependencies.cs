using Microsoft.Extensions.DependencyInjection;
using WaxRentals.Processing.Processors;

namespace WaxRentals.Processing.Config
{
    internal static class Dependencies
    {

        public static void AddDependencies(this IServiceCollection services)
        {
            services.AddSingleton<RentalOpenProcessor>();
            services.AddSingleton<RentalStakeProcessor>();
            services.AddSingleton<RentalSweepProcessor>();
            services.AddSingleton<RentalClosingProcessor>();

            services.AddSingleton<PurchaseProcessor>();

            services.AddSingleton<DayChangeProcessor>();

            services.AddSingleton<TrackBananoProcessor>();
            services.AddSingleton<TrackWaxProcessor>();

            services.AddSingleton<WelcomePackageOpenProcessor>();
            services.AddSingleton<WelcomePackageFundingProcessor>();
            services.AddSingleton<WelcomePackageNftProcessor>();
            services.AddSingleton<WelcomePackageRentalProcessor>();
            services.AddSingleton<WelcomePackageSweepProcessor>();

            services.AddSingleton<ClearOlderLogsProcessor>();
        }

    }
}
