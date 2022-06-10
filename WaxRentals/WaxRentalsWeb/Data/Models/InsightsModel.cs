using System.Collections.Generic;
using System.Linq;
using WaxRentals.Banano.Transact;

namespace WaxRentalsWeb.Data.Models
{
    public class InsightsModel
    {

        public IEnumerable<RentalModel> RecentRentals { get; }
        public IEnumerable<PurchaseModel> RecentPurchases { get; }
        public IEnumerable<WelcomePackageModel> RecentWelcomePackages { get; set; }
        public IEnumerable<MonthlyStatsModel> MonthlyStats { get; set; }

        public InsightsModel(Insights insights, IBananoAccountFactory banano)
        {
            RecentRentals         = insights.RecentRentals.Select(rental => new RentalModel(rental, banano));
            RecentPurchases       = insights.RecentPurchases.Select(purchase => new PurchaseModel(purchase));
            RecentWelcomePackages = insights.RecentWelcomePackages.Select(package => new WelcomePackageModel(package, banano));
            MonthlyStats          = insights.MonthlyStats.Select(stats => new MonthlyStatsModel(stats));
        }

    }
}
