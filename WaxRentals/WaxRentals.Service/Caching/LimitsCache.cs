using WaxRentals.Data.Manager;
using WaxRentals.Service.Caching.Values;
using WaxRentals.Service.Utilities;
using WaxConstants = WaxRentals.Waxp.Config.Constants.Protocol;
using BananoConstants = WaxRentals.Banano.Config.Constants.Protocol;

namespace WaxRentals.Service.Caching
{
    public class LimitsCache : CacheBase
    {

        public Limits GetLimits() => new Limits
        {
            BananoMinimumCredit = BananoConstants.Minimum,

            WaxMinimumRent = WaxConstants.MinimumTransaction,
            WaxMaximumRent = WaxInfo.GetBalances().Available / 2,

            WaxMinimumBuy = WaxConstants.MinimumTransaction,
            WaxMaximumBuy = Safe.Divide(BananoInfo.GetBalance(), Costs.GetCosts().WaxBuyPriceInBanano * 2)
        };
        

        private CostsCache Costs { get; }
        private BananoInfoCache BananoInfo { get; }
        private WaxInfoCache WaxInfo { get; }

        public LimitsCache(IDataFactory factory, CostsCache costs, BananoInfoCache bananoInfo, WaxInfoCache waxInfo)
            : base(factory)
        {
            Costs = costs;
            BananoInfo = bananoInfo;
            WaxInfo = waxInfo;
        }

    }
}
