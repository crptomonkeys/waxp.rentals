using System;
using WaxRentals.Monitoring.Utilities;
using BananoConstants = WaxRentals.Banano.Config.Constants.Protocol;
using Calculations = WaxRentals.Data.Config.Constants.Calculations;
using WaxConstants = WaxRentals.Waxp.Config.Constants.Protocol;

namespace WaxRentalsWeb.Data.Models
{
    public class AppState
    {

        public string SiteMessage { get; set; }
        public string WaxAccountToday { get; set; }
        public bool WelcomePackageNftsAvailable { get; set; }

        public VolatileDecimal BananoBalance { get; } = new();
        public VolatileDecimal WaxBalanceAvailable { get; } = new();
        public VolatileDecimal WaxBalanceStaked { get; } = new();
        public VolatileDecimal WaxBalanceUnstaking { get; } = new();

        public VolatileDecimal BananoPrice { get; } = new();
        public VolatileDecimal WaxPrice { get; } = new();

        public decimal WaxRentPriceInBanano => Calculations.BananoPerWaxPerDay;
        public decimal WaxBuyPriceInBanano => Safe.Divide(WaxPrice.Value, BananoPrice.Value);

        public decimal BananoMinimumCredit => BananoConstants.Minimum;
        // No BananoMaximumCredit because it's based on time, not number of WAX.
        public decimal WaxMinimumRent => WaxConstants.MinimumTransaction;
        public decimal WaxMaximumRent => WaxBalanceAvailable.Value >= (WaxMinimumRent * 2) ? (WaxBalanceAvailable.Value / 2) : WaxMinimumRent;
        public decimal WaxMinimumBuy => WaxConstants.MinimumTransaction;
        public decimal WaxMaximumBuy => Safe.Divide(BananoBalance.Value, WaxBuyPriceInBanano * 2);

        public decimal BananoWelcomePackagePrice => Math.Ceiling(WaxConstants.NewUser.ChargeWax * Safe.Divide(WaxPrice.Value, BananoPrice.Value));
        public decimal WaxWelcomePackageMinimumAvailable => WaxConstants.NewUser.OpenWax;

    }
}
