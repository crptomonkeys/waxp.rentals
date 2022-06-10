using System.Collections.Generic;
using WaxRentals.Data.Entities;

namespace WaxRentalsWeb.Data.Models
{
    public class Insights
    {

        public IEnumerable<Rental> RecentRentals { get; set; }
        public IEnumerable<Purchase> RecentPurchases { get; set; }
        public IEnumerable<WelcomePackage> RecentWelcomePackages { get; set; }
        public IEnumerable<MonthlyStats> MonthlyStats { get; set; }

    }
}
