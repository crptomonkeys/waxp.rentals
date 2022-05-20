using System;
using WaxRentals.Banano.Transact;
using WaxRentals.Data.Entities;

namespace WaxRentalsWeb.Data.Models
{
    public class RentalModel
    {

        public int Cpu { get; set; }
        public int Net { get; set; }
        public int Days { get; set; }
        public decimal Banano { get; set; }
        public string BananoAddress { get; set; }
        public string StakeTransaction { get; set; }

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
