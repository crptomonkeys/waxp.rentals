using WaxRentals.Service.Shared.Entities;

namespace WaxRentalsWeb.Data.Models
{
    public class RentalDetailModel
    {

        public BananoAddressModel Address { get; }
        public string Link { get; }
        public int Cpu { get; }
        public int Net { get; }
        public int Days { get; }
        public decimal Banano { get; }

        public RentalDetailModel(RentalInfo rental)
        {
            Address = new BananoAddressModel(rental.BananoAddress);
            Link    = rental.BananoPaymentLink;
            Cpu     = rental.Cpu;
            Net     = rental.Net;
            Days    = rental.Days;
            Banano  = decimal.Round(rental.Banano, 4);
        }

    }
}
