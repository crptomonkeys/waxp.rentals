using System.Collections.Generic;
using WaxRentals.Data.Entities;

namespace WaxRentals.Data.Manager
{
    public interface IExplore
    {

        IEnumerable<Rental> GetRecentRentals();
        IEnumerable<Purchase> GetRecentPurchases();
        Rental GetRentalByBananoAddress(string address);

    }
}
