using WaxRentals.Service.Shared.Entities;

namespace WaxRentalsWeb.Data.Models
{
    public class WelcomePackageModel
    {

        public decimal Banano { get; }
        public string BananoAddress { get; }
        public decimal Wax { get; }
        public string FundTransaction { get; }
        public string NftTransaction { get; }
        public string StakeTransaction { get; }

        public WelcomePackageModel(WelcomePackageInfo package)
        {
            Banano           = package.Banano;
            BananoAddress    = package.BananoAddress;
            Wax              = package.Wax;
            FundTransaction  = package.FundTransaction;
            NftTransaction   = package.NftTransaction;
            StakeTransaction = package.StakeTransaction;
        }

    }
}
