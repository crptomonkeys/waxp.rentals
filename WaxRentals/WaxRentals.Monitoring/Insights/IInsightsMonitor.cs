using System;
using System.Collections.Generic;
using WaxRentals.Data.Entities;

namespace WaxRentals.Monitoring.Recents
{
    public interface IInsightsMonitor
    {

        event EventHandler Updated;
        void Initialize();

        IEnumerable<Rental> RecentRentals { get; }
        IEnumerable<Purchase> RecentPurchases { get; }
        IEnumerable<WelcomePackage> RecentWelcomePackages { get; }
        IEnumerable<MonthlyStats> MonthlyStats { get; }

    }
}
