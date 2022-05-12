using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
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

        private ILog Log { get; }

        public EndpointMonitor(TimeSpan interval, ILog log)
            : base(interval)
        {
            Log = log;
        }
        
        protected override bool Tick(out Endpoints result)
        {
            result = new Endpoints();
            try
            {
                var json = JObject.Parse(new WebClient().DownloadString(Locations.Endpoints));
                result.Api = json.SelectTokens(Protocol.TransactionEndpoints)
                               .Select(token => token.Value<string>())
                               .Distinct(Comparer)
                               .ToList();
                result.History = json.SelectTokens(Protocol.HistoryEndpoints)
                                   .Select(token => token.Value<string>())
                                   .Distinct(Comparer)
                                   .ToList();
                return true;
            }
            catch (Exception ex)
            {
                Log.Error(ex);
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
