namespace WaxRentals.Service.Caching
{
    public sealed class Cache
    {

        public BananoInfoCache BananoInfo { get; }
        public CostsCache Costs { get; }
        public LimitsCache Limits { get; }
        public NftsCache Nfts { get; }
        public PricesCache Prices { get; }
        public WaxInfoCache WaxInfo { get; }

        public Cache(
            BananoInfoCache bananoInfo,
            CostsCache costs,
            LimitsCache limits,
            NftsCache nfts,
            PricesCache prices,
            WaxInfoCache waxInfo)
        {
            BananoInfo = bananoInfo;
            Costs = costs;
            Limits = limits;
            Nfts = nfts;
            Prices = prices;
            WaxInfo = waxInfo;
        }

    }
}
