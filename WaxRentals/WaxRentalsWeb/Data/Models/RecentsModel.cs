using System.Collections.Generic;
using System.Linq;
using WaxRentals.Banano.Transact;

namespace WaxRentalsWeb.Data.Models
{
    public class RecentsModel
    {

        public IEnumerable<RentalModel> Rentals { get; }
        public IEnumerable<PurchaseModel> Purchases { get; }

        public RecentsModel(Recents recents, IBananoAccountFactory banano)
        {
            Rentals = recents.Rentals.Select(rental => new RentalModel(rental, banano));
            Purchases = recents.Purchases.Select(purchase => new PurchaseModel(purchase));
        }

    }
}
