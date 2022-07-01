using System.Collections.Generic;
using System.Threading.Tasks;
using WaxRentals.Data.Entities;

namespace WaxRentals.Data.Manager
{
    public interface IExplore
    {

        Task<IEnumerable<Rental>> GetLatestRentals();
        Task<IEnumerable<Purchase>> GetLatestPurchases();
        Task<IEnumerable<WelcomePackage>> GetLatestWelcomePackages();
        Task<IEnumerable<MonthlyStats>> GetMonthlyStats();

        IEnumerable<Rental> GetRentalsByBananoAddresses(IEnumerable<string> addresses);
        IEnumerable<Rental> GetRentalsByWaxAccount(string account);
        IEnumerable<WelcomePackage> GetWelcomePackagesByBananoAddresses(IEnumerable<string> addresses);

    }
}
