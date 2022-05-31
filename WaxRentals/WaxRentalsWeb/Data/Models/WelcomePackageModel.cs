using WaxRentals.Banano.Transact;
using WaxRentals.Data.Entities;

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

        public WelcomePackageModel(WelcomePackage package, IBananoAccountFactory banano)
        {
            Banano           = package.Banano;
            BananoAddress    = banano.BuildWelcomeAccount((uint)package.PackageId).Address;
            Wax              = package.Wax;
            FundTransaction  = package.FundTransaction;
            NftTransaction   = package.NftTransaction;
            StakeTransaction = package.Rental?.StakeWaxTransaction;
        }

    }
}
