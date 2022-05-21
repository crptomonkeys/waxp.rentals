using System;
using WaxRentals.Banano.Transact;
using WaxRentals.Data.Entities;

namespace WaxRentalsWeb.Data.Models
{
    public class RentalModel
    {

        public int Cpu { get; }
        public int Net { get; }
        public int Days { get; }
        public decimal Banano { get; }
        public string BananoAddress { get; }
        public string StakeTransaction { get; }

        public RentalModel(Rental rental, IBananoAccountFactory banano)
        {
            Cpu              = Convert.ToInt32(rental.CPU);
            Net              = Convert.ToInt32(rental.NET);
            Days             = rental.RentalDays;
            Banano           = decimal.Round(rental.Banano, 4);
            BananoAddress    = banano.BuildAccount((uint)rental.RentalId).Address;
            StakeTransaction = rental.StakeWaxTransaction;
        }

    }
}
