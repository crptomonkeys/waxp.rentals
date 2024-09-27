using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;
using WaxRentals.Data.Manager;
using WaxRentals.Monitoring.Extensions;
using static WaxRentals.Monitoring.Config.Constants;
using static WaxRentals.Waxp.Config.Constants;
using Monitor = WaxRentals.Monitoring.Monitor;

namespace WaxRentals.Waxp.Monitoring
{
    internal class EndpointMonitor : Monitor
    {

        private Endpoints _endpoints;
        private readonly ReaderWriterLockSlim _rwls = new();
        public Endpoints Value { get { return _rwls.SafeRead(() => _endpoints); } }

        private HttpClient Client { get; }

        public EndpointMonitor(TimeSpan interval, ILog log)
            : base(interval, log)
        {
            Client = new HttpClient { Timeout = QuickTimeout };
        }
        
        protected async override Task<bool> Tick()
        {
            var update = false;

            try
            {
                var json = JObject.Parse(await Client.GetStringAsync(Locations.Endpoints));
                var endpoints = new Endpoints
                {
                    Api = json.SelectTokens(Protocol.TransactionEndpoints)
                              .Select(token => token.Value<string>())
                              .Distinct(Comparer)
                              .Where(endpoint => !Protocol.EndpointsBlacklist.Contains(endpoint, Comparer))
                              .ToList(),
                    History = json.SelectTokens(Protocol.HistoryEndpoints)
                                  .Select(token => token.Value<string>())
                                  .Distinct(Comparer)
                                  .Where(endpoint => !Protocol.EndpointsBlacklist.Contains(endpoint, Comparer))
                                  .ToList()
                };
                _rwls.SafeWrite(() => _endpoints = endpoints);
                update = true;
            }
            catch (Exception ex)
            {
                await Log.Error(ex, context: Locations.Endpoints);
            }

            return update;
        }

        public class Endpoints
        {
            public IEnumerable<string> Api { get; set; }
            public IEnumerable<string> History { get; set; }
        }

    }
}
