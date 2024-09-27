#nullable disable

namespace WaxRentals.Api.Entities.WelcomePackages
{
    public class WelcomePackageInfo
    {

        public BananoPaymentInfo Payment { get; set; }
        public WaxTargetInfo Target { get; set; }
        public TransactionsInfo Transactions { get; set; }
        public Status Status { get; set; }

    }
}
