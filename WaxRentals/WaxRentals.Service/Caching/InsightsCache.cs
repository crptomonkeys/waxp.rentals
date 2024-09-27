using WaxRentals.Data.Manager;
using WaxRentals.Monitoring.Extensions;
using WaxRentals.Service.Config;
using WaxRentals.Service.Shared.Entities;

namespace WaxRentals.Service.Caching
{
    public class InsightsCache : InvalidatableCache
    {

        public AppInsights GetInsights() => Rwls.SafeRead(() => Insights);


        private IExplore Explore { get; }
        private Mapper Mapper { get; }
        private ReaderWriterLockSlim Rwls { get; } = new();
        private AppInsights Insights { get; set; }

        public InsightsCache(ILog log, IExplore explore, TimeSpan interval, Mapper mapper)
            : base(log, interval)
        {
            Explore = explore;
            Mapper = mapper;

            // No nulls.
            Insights = new AppInsights
            {
                MonthlyStats = Enumerable.Empty<MonthlyStats>(),
                LatestRentals = Enumerable.Empty<RentalInfo>(),
                LatestPurchases = Enumerable.Empty<PurchaseInfo>(),
                LatestWelcomePackages = Enumerable.Empty<WelcomePackageInfo>()
            };
        }

        protected async override Task Tick()
        {
            var stats = Explore.GetMonthlyStats();
            var rentals = Explore.GetLatestRentals();
            var purchases = Explore.GetLatestPurchases();
            var packages = Explore.GetLatestWelcomePackages();
            await Task.WhenAll(stats, rentals, purchases, packages);

            Rwls.SafeWrite(async () =>
                Insights = new AppInsights
                {
                    MonthlyStats = (await stats).Select(Mapper.Map),
                    LatestRentals = (await rentals).Select(Mapper.Map),
                    LatestPurchases = (await purchases).Select(Mapper.Map),
                    LatestWelcomePackages = (await packages).Select(Mapper.Map)
                }
            );
        }

    }
}
