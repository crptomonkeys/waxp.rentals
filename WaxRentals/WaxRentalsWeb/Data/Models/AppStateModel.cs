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

        public AppStateModel(AppState state)
        {
            BananoBalance        = decimal.Round(state.BananoBalance.Value      , 4);
            WaxBalanceAvailable  = decimal.Round(state.WaxBalanceAvailable.Value, 0);
            WaxBalanceStaked     = decimal.Round(state.WaxBalanceStaked.Value   , 0);
            WaxBalanceUnstaking  = decimal.Round(state.WaxBalanceUnstaking.Value, 4);
            BananoPrice          = decimal.Round(state.BananoPrice.Value        , 4);
            WaxPrice             = decimal.Round(state.WaxPrice.Value           , 4);
            WaxRentPriceInBanano = decimal.Round(state.WaxRentPriceInBanano     , 4);
            WaxBuyPriceInBanano  = decimal.Round(state.WaxBuyPriceInBanano      , 4);
            BananoMinimumCredit  = decimal.Round(state.BananoMinimumCredit      , 4);
            WaxMinimumRent       = decimal.Round(state.WaxMinimumRent           , 0);
            WaxMaximumRent       = decimal.Round(state.WaxMaximumRent           , 0);
            WaxMinimumBuy        = decimal.Round(state.WaxMinimumBuy            , 0);
            WaxMaximumBuy        = decimal.Round(state.WaxMaximumBuy            , 0);
        }

    }
}
