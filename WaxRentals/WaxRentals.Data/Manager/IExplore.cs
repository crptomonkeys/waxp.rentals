using System.Collections.Generic;
using WaxRentals.Data.Entities;

namespace WaxRentals.Data.Manager
{
    public interface IExplore
    {

        IEnumerable<Rental> GetRecentRentals();
        IEnumerable<Purchase> GetRecentPurchases();
        IEnumerable<WelcomePackage> GetRecentWelcomePackages();
        IEnumerable<MonthlyStats> GetMonthlyStats();

        IEnumerable<Rental> GetRentalsByBananoAddresses(IEnumerable<string> addresses);
        IEnumerable<Rental> GetRentalsByWaxAccount(string account);
        IEnumerable<WelcomePackage> GetWelcomePackagesByBananoAddresses(IEnumerable<string> addresses);

    }
}
