using BananoConstants = WaxRentals.Banano.Config.Constants.Protocol;
using Calculations = WaxRentals.Data.Config.Constants.Calculations;
using WaxConstants = WaxRentals.Waxp.Config.Constants.Protocol;

namespace WaxRentalsWeb.Data
{
    public class AppState
    {

        public VolatileDecimal BananoBalance { get; } = new();
        public VolatileDecimal WaxBalanceAvailable { get; } = new();
        public VolatileDecimal WaxBalanceStaked { get; } = new();
        public VolatileDecimal WaxBalanceUnstaking { get; } = new();

        public VolatileDecimal BananoPrice { get; } = new();
        public VolatileDecimal WaxPrice { get; } = new();

        public decimal WaxRentPriceInBanano { get { return Calculations.BananoPerWaxPerDay; } }
        public decimal WaxBuyPriceInBanano { get { return BananoPrice.Value / WaxPrice.Value; } }

        public decimal BananoMinimumCredit { get { return BananoConstants.Minimum; } }
        // No BananoMaximumCredit because it's based on time, not number of WAX.
        public decimal WaxMinimumRent { get { return WaxConstants.MinimumTransaction; } }
        public decimal WaxMaximumRent { get { return WaxBalanceAvailable.Value >= (WaxMinimumRent * 2) ? (WaxBalanceAvailable.Value / 2) : WaxMinimumRent; } }
        public decimal WaxMinimumBuy { get { return WaxConstants.MinimumTransaction; } }
        public decimal WaxMaximumBuy { get { return BananoBalance.Value / (WaxBuyPriceInBanano * 2); } }

    }
}
