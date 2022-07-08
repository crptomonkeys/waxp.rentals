namespace WaxRentals.Service.Caching.Values
{
    public class Limits
    {

        public decimal BananoMinimumCredit { get; internal set; }
        // No BananoMaximumCredit because it's based on time, not number of WAX.

        public decimal WaxMinimumRent { get; internal set; }
        public decimal WaxMaximumRent { get; internal set; }

        public decimal WaxMinimumBuy { get; internal set; }
        public decimal WaxMaximumBuy { get; internal set; }

    }
}
