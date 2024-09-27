namespace WaxRentals.Service.Caching
{
    public sealed class Cache
    {

        public BananoInfoCache BananoInfo { get; }
        public CostsCache Costs { get; }
        public InsightsCache Insights { get; }
        public LimitsCache Limits { get; }
        public NftsCache Nfts { get; }
        public PricesCache Prices { get; }
        public WaxInfoCache WaxInfo { get; }

        public Cache(
            BananoInfoCache bananoInfo,
            CostsCache costs,
            InsightsCache insights,
            LimitsCache limits,
            NftsCache nfts,
            PricesCache prices,
            WaxInfoCache waxInfo)
        {
            BananoInfo = bananoInfo;
            Costs = costs;
            Insights = insights;
            Limits = limits;
            Nfts = nfts;
            Prices = prices;
            WaxInfo = waxInfo;
        }

    }
}
