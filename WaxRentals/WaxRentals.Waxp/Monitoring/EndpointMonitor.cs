using System;
using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json.Linq;
using WaxRentals.Data.Manager;
using WaxRentals.Monitoring;
using static WaxRentals.Monitoring.Config.Constants;
using static WaxRentals.Waxp.Config.Constants;
using static WaxRentals.Waxp.Monitoring.EndpointMonitor;

namespace WaxRentals.Waxp.Monitoring
{
    internal class EndpointMonitor : Monitor<Endpoints>
    {

        public EndpointMonitor(TimeSpan interval, ILog log) : base(interval, log) { }
        
        protected override bool Tick(out Endpoints result)
        {
            result = new Endpoints();
            try
            {
                var json = JObject.Parse(new QuickTimeoutWebClient().DownloadString(Locations.Endpoints, QuickTimeout));
                result.Api = json.SelectTokens(Protocol.TransactionEndpoints)
                               .Select(token => token.Value<string>())
                               .Distinct(Comparer)
                               .Where(endpoint => !Protocol.EndpointsBlacklist.Contains(endpoint, Comparer))
                               .ToList();
                result.History = json.SelectTokens(Protocol.HistoryEndpoints)
                                   .Select(token => token.Value<string>())
                                   .Distinct(Comparer)
                                   .Where(endpoint => !Protocol.EndpointsBlacklist.Contains(endpoint, Comparer))
                                   .ToList();
                return true;
            }
            catch (Exception ex)
            {
                Log.Error(ex, context: Locations.Endpoints);
                return false;
            }
        }

        public class Endpoints
        {
            public IEnumerable<string> Api { get; set; }
            public IEnumerable<string> History { get; set; }
        }

    }
}
