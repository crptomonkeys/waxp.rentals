#nullable disable

namespace WaxRentals.Api.Entities.Rentals
{
    public class RentalInfo
    {

        public BananoPaymentInfo Payment { get; set; }
        public WaxTargetInfo Target { get; set; }
        public DatesInfo Dates { get; set; }
        public TransactionsInfo Transactions { get; set; }
        public Status Status { get; set; }

    }
}
