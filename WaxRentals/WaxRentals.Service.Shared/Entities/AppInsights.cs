using System.Collections.Generic;

#nullable disable

namespace WaxRentals.Service.Shared.Entities
{
    public class AppInsights
    {

        public IEnumerable<MonthlyStats> MonthlyStats { get; set; }
        public IEnumerable<RentalInfo> LatestRentals { get; set; }
        public IEnumerable<PurchaseInfo> LatestPurchases { get; set; }
        public IEnumerable<WelcomePackageInfo> LatestWelcomePackages { get; set; }

    }
}
