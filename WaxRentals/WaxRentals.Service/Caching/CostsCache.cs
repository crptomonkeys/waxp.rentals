using WaxRentals.Data.Manager;
using WaxRentals.Service.Caching.Values;
using WaxRentals.Service.Shared.Utilities;
using static WaxRentals.Service.Config.Constants;
using static WaxRentals.Service.Shared.Config.Constants.Wax;

namespace WaxRentals.Service.Caching
{
    public class CostsCache : CacheBase
    {

        public Costs GetCosts()
        {
            var prices = Prices.GetPrices();
            return new Costs
            {
                WaxRentPriceInBanano = Calculations.BananoPerWaxPerDay,
                WaxBuyPriceInBanano = Safe.Divide(prices.Wax, prices.Banano),
                BananoWelcomePackagePrice = Math.Ceiling(NewUser.ChargeWax * Safe.Divide(prices.Wax, prices.Banano))
            };
        }

        private PricesCache Prices { get; }

        public CostsCache(IDataFactory factory, PricesCache prices)
            : base(factory)
        {
            Prices = prices;
        }

    }
}
