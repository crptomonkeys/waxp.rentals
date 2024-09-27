﻿using WaxRentals.Data.Manager;
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
        private LockedDecimal Banano { get; } = new LockedDecimal();
        private LockedDecimal Wax { get; } = new LockedDecimal();

        public PricesCache(ILog log, TimeSpan interval, HttpClient client)
            : base(log, interval)
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
