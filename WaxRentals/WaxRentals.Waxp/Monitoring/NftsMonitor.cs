using System;
using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json.Linq;
using WaxRentals.Data.Manager;
using WaxRentals.Monitoring;
using WaxRentals.Waxp.Transact;
using static WaxRentals.Monitoring.Config.Constants;
using static WaxRentals.Waxp.Config.Constants;

namespace WaxRentals.Waxp.Monitoring
{
    public class NftsMonitor : Monitor<IEnumerable<Nft>>
    {

        public NftsMonitor(TimeSpan interval, IDataFactory factory) : base(interval, factory) { }

        protected override bool Tick(out IEnumerable<Nft> result)
        {
            try
            {
                var json = JObject.Parse(new QuickTimeoutWebClient().DownloadString(Locations.Assets, QuickTimeout));
                result = json.SelectTokens(Protocol.Assets)
                             .Select(token => token.ToObject<Nft>())
                             .ToList();
                return true;
            }
            catch (Exception ex)
            {
                Factory.Log.Error(ex, context: Locations.Assets);
                result = Enumerable.Empty<Nft>();
                return false;
            }
        }

    }
}
