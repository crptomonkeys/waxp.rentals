using WaxRentals.Service.Shared.Entities;

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

        public RentalModel(RentalInfo rental)
        {
            Cpu              = rental.Cpu;
            Net              = rental.Net;
            Days             = rental.Days;
            Banano           = decimal.Round(rental.Banano, 4);
            BananoAddress    = rental.BananoAddress;
            StakeTransaction = rental.StakeTransaction;
        }

    }
}
