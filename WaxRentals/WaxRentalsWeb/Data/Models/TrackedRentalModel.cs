using System;
using WaxRentals.Service.Shared.Entities;

namespace WaxRentalsWeb.Data.Models
{
    public class TrackedRentalModel
    {

        public string WaxAccount { get; }
        public int Cpu { get; }
        public int Net { get; }
        public int Days { get; }
        public decimal Banano { get; }
        public string BananoAddress { get; }
        public DateTime? Paid { get; }
        public DateTime? Expires { get; }
        public string StakeTransaction { get; }
        public string UnstakeTransaction { get; }
        public Status Status { get; }

        public TrackedRentalModel(RentalInfo rental)
        {
            WaxAccount         = rental.WaxAccount;
            Cpu                = rental.Cpu;
            Net                = rental.Net;
            Days               = rental.Days;
            Banano             = decimal.Round(rental.Banano, 4);
            BananoAddress      = rental.BananoAddress;
            Paid               = rental.Paid;
            Expires            = rental.Expires;
            StakeTransaction   = rental.StakeTransaction;
            UnstakeTransaction = rental.UnstakeTransaction;
            Status             = rental.Status;
        }

    }
}
