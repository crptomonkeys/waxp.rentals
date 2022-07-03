using WaxRentals.Data.Manager;
using WaxRentals.Service.Caching.Values;
using static WaxRentals.Service.Shared.Config.Constants;

namespace WaxRentals.Service.Caching
{
    public class PricesCache : TimedCacheBase
    {

        public Prices GetPrices() => new()
        {
            Banano = Banano.Value,
            Wax = Wax.Value
        };

        private HttpClient Client { get; }
        private VolatileDecimal Banano { get; } = new VolatileDecimal();
        private VolatileDecimal Wax { get; } = new VolatileDecimal();

        public PricesCache(IDataFactory factory, TimeSpan interval, HttpClient client)
            : base(factory, interval)
        {
            Client = client;
        }

        protected async override Task Tick()
        {
            var prices = await Client.GetFromJsonAsync<IDictionary<string, Price>>(null as Uri);
            if (prices?.TryGetValue(Coins.Banano, out Price? banano) ?? false)
            {
                Banano.Value = banano.usd;
            }
            if (prices?.TryGetValue(Coins.Wax, out Price? wax) ?? false)
            {
                Wax.Value = wax.usd;
            }
        }

        private class Price
        {
            public decimal usd { get; set; }
        }

    }
}
