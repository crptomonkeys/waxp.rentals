using System;
using WaxRentals.Banano.Transact;
using WaxRentals.Data.Entities;

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

        public RentalDetailModel(Rental rental, IBananoAccountFactory banano)
        {
            var account = banano.BuildAccount(rental.RentalId);
            Address = new BananoAddressModel(account.Address);
            Link = account.BuildLink(rental.Banano);
            Cpu = Convert.ToInt32(rental.CPU);
            Net = Convert.ToInt32(rental.NET);
            Days = rental.RentalDays;
            Banano = decimal.Round(rental.Banano, 4);
        }

    }
}
