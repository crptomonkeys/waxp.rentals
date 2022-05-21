using System;
using WaxRentals.Banano.Transact;
using WaxRentals.Data.Entities;

namespace WaxRentalsWeb.Data.Models
{
    public class RentalDetailModel
    {

        public AddressModel Address { get; }
        public string Link { get; }
        public int Cpu { get; }
        public int Net { get; }
        public int Days { get; }
        public decimal Banano { get; }

        public RentalDetailModel(Rental rental, IBananoAccountFactory banano)
        {
            var account = banano.BuildAccount((uint)rental.RentalId);
            Address = new AddressModel(account.Address);
            Link = account.BuildLink(rental.Banano);
            Cpu = Convert.ToInt32(rental.CPU);
            Net = Convert.ToInt32(rental.NET);
            Days = rental.RentalDays;
            Banano = decimal.Round(rental.Banano, 4);
        }

        public class AddressModel
        {
            public string Full { get; }
            public string Start { get; }
            public string Mid { get; }
            public string End { get; }

            internal AddressModel(string address)
            {
                Full = address;
                Start = address[..11];
                Mid = address[11..58];
                End = address[58..];
            }
        }

    }
}
