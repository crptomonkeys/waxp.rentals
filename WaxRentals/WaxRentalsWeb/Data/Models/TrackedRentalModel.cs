using System;
using WaxRentals.Banano.Transact;
using WaxRentals.Data.Entities;

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

        public TrackedRentalModel(Rental rental, IBananoAccountFactory banano)
        {
            WaxAccount         = rental.TargetWaxAccount;
            Cpu                = Convert.ToInt32(rental.CPU);
            Net                = Convert.ToInt32(rental.NET);
            Days               = rental.RentalDays;
            Banano             = decimal.Round(rental.Banano, 4);
            BananoAddress      = banano.BuildAccount((uint)rental.RentalId).Address;
            Paid               = rental.Paid;
            Expires            = rental.PaidThrough;
            StakeTransaction   = rental.StakeWaxTransaction;
            UnstakeTransaction = rental.UnstakeWaxTransaction;
            Status             = rental.Status;
        }

    }
}
