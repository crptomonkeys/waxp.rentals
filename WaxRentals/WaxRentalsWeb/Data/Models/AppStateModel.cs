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
        public decimal WaxWelcomePackageMinimumAvailable { get; }
        public bool WelcomePackageNftsAvailable { get; }
        public bool WelcomePackageRentalsAvailable { get; }
        public string SiteMessage { get; }
        public string WaxAccountToday { get; }

        public AppStateModel(Data.Models.AppState oldState, WaxRentals.Service.Shared.Entities.AppState state)
        {
            BananoBalance                     = decimal.Round(   state.BananoBalance                    , 4);
            WaxBalanceAvailable               = decimal.Floor(   state.WaxBalanceAvailableToday            );
            WaxBalanceStaked                  = decimal.Floor(   state.WaxStaked                           );
            WaxBalanceUnstaking               = decimal.Floor(   state.WaxBalanceAvailableTomorrow         );
            BananoPrice                       = decimal.Round(   state.BananoPrice                      , 4);
            WaxPrice                          = decimal.Round(   state.WaxPrice                         , 4);
            WaxRentPriceInBanano              = decimal.Round(oldState.WaxRentPriceInBanano             , 4);
            WaxBuyPriceInBanano               = decimal.Round(oldState.WaxBuyPriceInBanano              , 4);
            BananoMinimumCredit               = decimal.Round(oldState.BananoMinimumCredit              , 4);
            WaxMinimumRent                    = decimal.Round(oldState.WaxMinimumRent                   , 0);
            WaxMaximumRent                    = decimal.Round(oldState.WaxMaximumRent                   , 0);
            WaxMinimumBuy                     = decimal.Round(oldState.WaxMinimumBuy                    , 0);
            WaxMaximumBuy                     = decimal.Round(oldState.WaxMaximumBuy                    , 0);
            BananoWelcomePackagePrice         = decimal.Round(oldState.BananoWelcomePackagePrice        , 0);
            WaxWelcomePackageMinimumAvailable = decimal.Round(oldState.WaxWelcomePackageMinimumAvailable, 0);
            WelcomePackageNftsAvailable       =               oldState.WelcomePackageNftsAvailable          ;
            WelcomePackageRentalsAvailable    =               oldState.WelcomePackageRentalsAvailable       ;
            SiteMessage                       =               oldState.SiteMessage                          ;
            WaxAccountToday                   =                  state.WaxWorkingAccount                    ;
        }

    }
}
