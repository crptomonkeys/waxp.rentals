#nullable disable

namespace WaxRentals.Service.Shared.Entities
{
    public class WelcomePackageInfo
    {

        public decimal Banano { get; set; }
        public string BananoAddress { get; set; }
        public string BananoPaymentLink { get; set; }
        public decimal Wax { get; set; }
        public string FundTransaction { get; set; }
        public string NftTransaction { get; set; }
        public string StakeTransaction { get; set; }
        public Status Status { get; set; }

    }
}
