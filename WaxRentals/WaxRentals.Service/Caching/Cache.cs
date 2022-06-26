namespace WaxRentals.Service.Caching
{
    public sealed class Cache
    {

        public BananoInfoCache BananoInfo { get; }
        public CostsCache Costs { get; }
        public LimitsCache Limits { get; }
        public PricesCache Prices { get; }
        public WaxInfoCache WaxInfo { get; }

        public Cache(
            BananoInfoCache bananoInfo,
            CostsCache costs,
            LimitsCache limits,
            PricesCache prices,
            WaxInfoCache waxInfo)
        {
            BananoInfo = bananoInfo;
            Costs = costs;
            Limits = limits;
            Prices = prices;
            WaxInfo = waxInfo;
        }

    }
}
