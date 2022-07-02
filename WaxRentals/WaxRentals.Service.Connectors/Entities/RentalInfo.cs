using System;

#nullable disable

namespace WaxRentals.Service.Shared.Entities
{
    public class RentalInfo
	{

        public string WaxAccount { get; set; }
        public int Cpu { get; set; }
        public int Net { get; set; }
        public int Days { get; set; }
        public decimal Banano { get; set; }
        public string BananoAddress { get; set; }
        public string BananoPaymentLink { get; set; }
        public DateTime? Paid { get; set; }
        public DateTime? Expires { get; set; }
        public string StakeTransaction { get; set; }
        public string UnstakeTransaction { get; set; }
        public Status Status { get; set; }

    }
}
