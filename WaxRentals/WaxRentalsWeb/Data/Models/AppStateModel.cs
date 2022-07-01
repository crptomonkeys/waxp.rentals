using State = WaxRentals.Service.Shared.Entities.AppState;

namespace WaxRentalsWeb.Data.Models
{
    public class AppStateModel
    {

        public decimal BananoBalance { get; }
        public decimal WaxBalanceAvailable { get; }
        public decimal WaxBalanceStaked { get; }
        public decimal WaxBalanceUnstaking { get; }
        public decimal BananoPrice { get; }
        public decimal WaxPrice { get; }
        public decimal WaxRentPriceInBanano { get; }
        public decimal WaxBuyPriceInBanano { get; }
        public decimal BananoMinimumCredit { get; }
        public decimal WaxMinimumRent { get; }
        public decimal WaxMaximumRent { get; }
        public decimal WaxMinimumBuy { get; }
        public decimal WaxMaximumBuy { get; }
        public decimal BananoWelcomePackagePrice { get; }
        public bool WelcomePackagesAvailable { get; }
        public bool WelcomePackageNftsAvailable { get; }
        public bool WelcomePackageRentalsAvailable { get; }
        public string SiteMessage { get; }
        public string WaxAccountToday { get; }

        public AppStateModel(State state, string siteMessage)
        {
            BananoBalance                  = decimal.Round(state.BananoBalance                 , 4);
            WaxBalanceAvailable            = decimal.Floor(state.WaxBalanceAvailableToday         );
            WaxBalanceStaked               = decimal.Floor(state.WaxStaked                        );
            WaxBalanceUnstaking            = decimal.Floor(state.WaxBalanceAvailableTomorrow      );
            BananoPrice                    = decimal.Round(state.BananoPrice                   , 4);
            WaxPrice                       = decimal.Round(state.WaxPrice                      , 4);
            WaxRentPriceInBanano           = decimal.Round(state.WaxRentPriceInBanano          , 4);
            WaxBuyPriceInBanano            = decimal.Round(state.WaxBuyPriceInBanano           , 4);
            BananoMinimumCredit            = decimal.Round(state.BananoMinimumCredit           , 4);
            WaxMinimumRent                 = decimal.Round(state.WaxMinimumRent                , 0);
            WaxMaximumRent                 = decimal.Round(state.WaxMaximumRent                , 0);
            WaxMinimumBuy                  = decimal.Round(state.WaxMinimumBuy                 , 0);
            WaxMaximumBuy                  = decimal.Round(state.WaxMaximumBuy                 , 0);
            BananoWelcomePackagePrice      = decimal.Round(state.BananoWelcomePackagePrice     , 0);
            WelcomePackagesAvailable       =               state.WelcomePackagesAvailable          ;
            WelcomePackageNftsAvailable    =               state.WelcomePackageNftsAvailable       ;
            WelcomePackageRentalsAvailable =               state.WelcomePackageRentalsAvailable    ;
            SiteMessage                    =                     siteMessage                       ;
            WaxAccountToday                =               state.WaxWorkingAccount                 ;
        }

    }
}
