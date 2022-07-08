using System.Collections.Generic;
using System.Linq;
using WaxRentals.Service.Shared.Entities;

namespace WaxRentalsWeb.Data.Models
{
    public class AppInsightsModel
    {

        public IEnumerable<MonthlyStatsModel> MonthlyStats { get; set; }
        public IEnumerable<RentalModel> LatestRentals { get; }
        public IEnumerable<PurchaseModel> LatestPurchases { get; }
        public IEnumerable<WelcomePackageModel> LatestWelcomePackages { get; set; }

        public AppInsightsModel(AppInsights insights)
        {
            MonthlyStats          = insights.MonthlyStats.Select(stats => new MonthlyStatsModel(stats));
            LatestRentals         = insights.LatestRentals.Select(rental => new RentalModel(rental));
            LatestPurchases       = insights.LatestPurchases.Select(purchase => new PurchaseModel(purchase));
            LatestWelcomePackages = insights.LatestWelcomePackages.Select(package => new WelcomePackageModel(package));
        }

    }
}
