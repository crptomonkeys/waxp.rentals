using WaxRentals.Api.Entities.Rentals;
using WaxRentals.Api.Entities.WelcomePackages;

#nullable disable

namespace WaxRentals.Api.Entities.App
{
    public class AppInsights
    {

        public IEnumerable<MonthlyStats> MonthlyStats { get; set; }
        public IEnumerable<RentalInfo> LatestRentals { get; set; }
        public IEnumerable<PurchaseInfo> LatestPurchases { get; set; }
        public IEnumerable<WelcomePackageInfo> LatestWelcomePackages { get; set; }

    }
}
