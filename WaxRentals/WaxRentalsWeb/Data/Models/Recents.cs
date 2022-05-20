using System.Collections.Generic;
using WaxRentals.Data.Entities;

namespace WaxRentalsWeb.Data.Models
{
    public class Recents
    {

        public IEnumerable<Rental> Rentals { get; set; }
        public IEnumerable<Purchase> Purchases { get; set; }

    }
}
