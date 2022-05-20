using System;
using System.Collections.Generic;
using System.Net;
using System.Threading;
using Newtonsoft.Json;
using WaxRentals.Data.Manager;
using WaxRentals.Monitoring.Extensions;
using static WaxRentals.Monitoring.Config.Constants;

namespace WaxRentals.Monitoring.Prices
{
    internal class PriceMonitor : Monitor, IPriceMonitor
    {

        private readonly string _url;

        private readonly ReaderWriterLockSlim _bananoLock = new();
        private readonly ReaderWriterLockSlim _waxLock = new();

        private decimal _banano, _wax;
        public decimal Banano { get { return _bananoLock.SafeRead(() => _banano); } }
        public decimal Wax { get { return _waxLock.SafeRead(() => _wax); } }

        public PriceMonitor(TimeSpan interval, IDataFactory factory, string url)
            : base(interval, factory)
        {
            _url = url;
        }

        protected override bool Tick()
        {
            var update = false;

            try
            {
                var data = new WebClient().DownloadString(_url);
                var parsed = JsonConvert.DeserializeObject<IDictionary<string, Price>>(data);

                if (parsed.TryGetValue(Coins.Banano, out Price banano))
                {
                    if (_banano != banano.usd)
                    {
                        update = true;
                        _bananoLock.SafeWrite(() => _banano = banano.usd);
                    }
                }

                if (parsed.TryGetValue(Coins.Wax, out Price wax))
                {
                    if (_wax != wax.usd)
                    {
                        update = true;
                        _waxLock.SafeWrite(() => _wax = wax.usd);
                    }
                }
            }
            catch (Exception ex)
            {
                Factory.Log.Error(ex);
            }

            return update;
        }

        private class Price
        {
            public decimal usd { get; set; }
        }

    }
}
