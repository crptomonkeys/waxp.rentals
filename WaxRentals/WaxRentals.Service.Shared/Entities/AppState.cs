#nullable disable

namespace WaxRentals.Service.Shared.Entities
{
    public class AppState
    {

        // Banano

        public decimal BananoPrice { get; set; }
        public string BananoAddress { get; set; }
        public decimal BananoBalance { get; set; }
        
        // Wax

        public decimal WaxPrice { get; set; }
        public string WaxAccount { get; set; }
        public decimal WaxStaked { get; set; }
        public string WaxWorkingAccount { get; set; }
        public decimal WaxBalanceAvailableToday { get; set; }
        public decimal WaxBalanceAvailableTomorrow { get; set; }

        // Costs

        public decimal WaxRentPriceInBanano { get; set; }
        public decimal WaxBuyPriceInBanano { get; set; }
        public decimal BananoWelcomePackagePrice { get; set; }

        // Limits

        public decimal BananoMinimumCredit { get; set; }
        public decimal WaxMinimumRent { get; set; }
        public decimal WaxMaximumRent { get; set; }
        public decimal WaxMinimumBuy { get; set; }
        public decimal WaxMaximumBuy { get; set; }

        // Welcome Packages

        public bool WelcomePackagesAvailable { get; set; }
        public bool WelcomePackageRentalsAvailable { get; set; }
        public bool WelcomePackageNftsAvailable { get; set; }

    }
}
