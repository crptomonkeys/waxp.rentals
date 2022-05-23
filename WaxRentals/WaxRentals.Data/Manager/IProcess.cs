using System.Collections.Generic;
using System.Threading.Tasks;
using WaxRentals.Data.Entities;

namespace WaxRentals.Data.Manager
{
    public interface IProcess
    {

        Task<IEnumerable<Rental>> PullNewRentals();
        Task ProcessRentalPayment(int rentalId);
        
        Task<IEnumerable<Rental>> PullPaidRentalsToStake();
        Task ProcessRentalStaking(int rentalId, string source, string transaction);

        Task<IEnumerable<Rental>> PullSweepableRentals();
        Task ProcessRentalSweep(int rentalId, string transaction);

        Task<Rental> PullNextClosingRental();
        Task ProcessRentalClosing(int rentalId, string transaction);

        Task<Purchase> PullNextPurchase();
        Task ProcessPurchase(int purchaseId, string transaction);

    }
}
