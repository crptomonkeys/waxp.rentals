using WaxRentals.Data.Manager;
using WaxRentals.Monitoring.Extensions;
using WaxRentals.Service.Config;
using WaxRentals.Service.Shared.Entities;

namespace WaxRentals.Service.Caching
{
    public class InsightsCache : InvalidatableCache
    {

        public AppInsights GetInsights() => Rwls.SafeRead(() => Insights);


        private Mapper Mapper { get; }
        private ReaderWriterLockSlim Rwls { get; } = new();
        private AppInsights Insights { get; set; } = new();

        public InsightsCache(IDataFactory factory, TimeSpan interval, Mapper mapper)
            : base(factory, interval)
        {
            Mapper = mapper;
        }

        protected async override Task Tick()
        {
            var stats = Factory.Explore.GetMonthlyStats();
            var rentals = Factory.Explore.GetLatestRentals();
            var purchases = Factory.Explore.GetLatestPurchases();
            var packages = Factory.Explore.GetLatestWelcomePackages();
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
