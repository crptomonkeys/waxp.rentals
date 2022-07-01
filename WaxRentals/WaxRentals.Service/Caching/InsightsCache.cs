using WaxRentals.Banano.Transact;
using WaxRentals.Data.Manager;
using WaxRentals.Monitoring.Extensions;
using WaxRentals.Service.Shared.Entities;

namespace WaxRentals.Service.Caching
{
    public class InsightsCache : InvalidatableCache
    {

        public AppInsights GetInsights() => Rwls.SafeRead(() => Insights);


        private IBananoAccountFactory Banano { get; }
        private ReaderWriterLockSlim Rwls { get; } = new();
        private AppInsights Insights { get; set; } = new();

        public InsightsCache(IDataFactory factory, TimeSpan interval, IBananoAccountFactory banano)
            : base(factory, interval)
        {
            Banano = banano;
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
                    MonthlyStats = (await stats).Select(Map),
                    LatestRentals = (await rentals).Select(Map),
                    LatestPurchases = (await purchases).Select(Map),
                    LatestWelcomePackages = (await packages).Select(Map)
                }
            );
        }

        private static MonthlyStats Map(Data.Entities.MonthlyStats stats)
        {
            return new MonthlyStats
            {
                Year                  = stats.Year,
                Month                 = stats.Month,
                WaxDaysRented         = stats.WaxDaysRented,
                WaxDaysFree           = stats.WaxDaysFree,
                WaxPurchasedForSite   = stats.WaxPurchasedForSite,
                WelcomePackagesOpened = stats.WelcomePackagesOpened
            };
        }

        private RentalInfo Map(Data.Entities.Rental rental)
        {
            return new RentalInfo
            {
                WaxAccount         = rental.TargetWaxAccount,
                Cpu                = Convert.ToInt32(rental.CPU),
                Net                = Convert.ToInt32(rental.NET),
                Days               = rental.RentalDays,
                Banano             = rental.Banano,
                BananoAddress      = Banano.BuildAccount(rental.RentalId).Address,
                Paid               = rental.Paid,
                Expires            = rental.PaidThrough,
                StakeTransaction   = rental.StakeWaxTransaction,
                UnstakeTransaction = rental.UnstakeWaxTransaction,
                Status             = Map(rental.Status)
            };
        }

        private static PurchaseInfo Map(Data.Entities.Purchase purchase)
        {
            return new PurchaseInfo
            {
                Wax               = purchase.Wax,
                WaxTransaction    = purchase.WaxTransaction,
                Banano            = purchase.Banano,
                BananoTransaction = purchase.BananoTransaction
            };
        }

        private WelcomePackageInfo Map(Data.Entities.WelcomePackage package)
        {
            return new WelcomePackageInfo
            {
                Banano           = package.Banano,
                BananoAddress    = Banano.BuildWelcomeAccount(package.PackageId).Address,
                Wax              = package.Wax,
                FundTransaction  = package.FundTransaction,
                NftTransaction   = package.NftTransaction,
                StakeTransaction = package.Rental?.StakeWaxTransaction
            };
        }

        private Status Map(Data.Entities.Status status)
        {
            return Enum.Parse<Status>(status.ToString());
        }

    }
}
