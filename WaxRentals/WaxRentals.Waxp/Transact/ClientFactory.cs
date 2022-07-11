using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using Eos.Api;
using Newtonsoft.Json.Linq;
using WaxRentals.Data.Manager;
using WaxRentals.Monitoring.Extensions;
using WaxRentals.Monitoring.Logging;
using WaxRentals.Waxp.Monitoring;
using static WaxRentals.Monitoring.Config.Constants;

namespace WaxRentals.Waxp.Transact
{
    internal class ClientFactory : IClientFactory
    {

        #region " Endpoints "

        // Start with defaults, just in case the endpoint list is down.
        private readonly IDictionary<string, Status> _api = new Dictionary<string, Status>()
        {
            { "https://apiwax.3dkrender.com"          , new() },
            { "https://query.3dkrender.com"           , new() },
            { "https://api.wax.alohaeos.com"          , new() },
            { "https://wax.eu.eosamsterdam.net"       , new() },
            { "https://wax.blacklusion.io"            , new() },
            { "https://hyperion.wax.blacklusion.io"   , new() },
            { "https://api-wax-mainnet.wecan.dev"     , new() },
            { "https://hyperion-wax-mainnet.wecan.dev", new() },
            { "https://history-wax-mainnet.wecan.dev" , new() },
            { "https://wax.dapplica.io"               , new() },
            { "https://api-wax.eosauthority.com"      , new() },
            { "https://wax.eosdac.io"                 , new() },
            { "https://wax.eosdublin.io"              , new() },
            { "https://wax.eoseoul.io"                , new() },
            { "https://wax.eosphere.io"               , new() },
            { "https://wax-public1.neftyblocks.com"   , new() },
            { "https://wax-public2.neftyblocks.com"   , new() },
            { "https://waxapi.ledgerwise.io"          , new() },
            { "https://wax.api.eosnation.io"          , new() },
            { "https://wax.dfuse.eosnation.io"        , new() },
            { "https://wax.greymass.com"              , new() },
            { "https://api.waxeastern.cn"             , new() },
            { "https://api2.hivebp.io"                , new() },
            { "https://wax-api.eosiomadrid.io"        , new() },
            { "https://api.waxsweden.org"             , new() },
            { "https://wax-bp.wizardsguild.one"       , new() }
        };
        private readonly IDictionary<string, Status> _history = new Dictionary<string, Status>()
        {
            { "https://apiwax.3dkrender.com"         , new() },
            { "https://wax.eu.eosamsterdam.net"      , new() },
            { "https://history-wax-mainnet.wecan.dev", new() },
            { "https://api-wax.eosauthority.com"     , new() },
            { "https://wax.eosdublin.io"             , new() },
            { "https://api.waxeastern.cn"            , new() }
        };
        private readonly Random _random = new();

        public ClientFactory(EndpointMonitor monitor, ILog log)
        {
            Log = log;

            monitor.Updated += (_, _) =>
            {
                Update(_api, monitor.Value.Api);
                Update(_history, monitor.Value.History);
            };
            monitor.Initialize();
        }

        private void Update(IDictionary<string, Status> endpoints, IEnumerable<string> found)
        {
            if (found.Any())
            {
                _locker.SafeWrite(() =>
                {
                    foreach (var endpoint in endpoints.Keys.Except(found, Comparer))
                    {
                        endpoints.Remove(endpoint);
                    }
                    foreach (var endpoint in found.Except(endpoints.Keys, Comparer))
                    {
                        endpoints[endpoint] = new Status();
                    }
                });
            }
        }

        private (string, Status) GetEndpoint(IDictionary<string, Status> endpoints)
        {
            var kvp = _locker.SafeRead(() =>
            {
                var available = endpoints.Where(kvp => kvp.Value.Available < DateTime.UtcNow);
                if (available.Any())
                {
                    return GetRandom(available);
                }
                return GetRandom(endpoints);
            });
            return (kvp.Key, kvp.Value);
        }

        private KeyValuePair<string, Status> GetRandom(IEnumerable<KeyValuePair<string, Status>> endpoints)
        {
            return endpoints.ElementAt(_random.Next(endpoints.Count()));
        }

        private class Status
        {
            private ushort _failures = 0;
            public ushort Failures
            {
                get { return _locker.SafeRead(() => _failures); }
            }

            private DateTime _available = DateTime.MinValue;
            public DateTime Available
            {
                get { return _locker.SafeRead(() => _available); }
            }

            public void Fail()
            {
                _locker.SafeWrite(() =>
                {
                    _available = DateTime.UtcNow.AddHours(++_failures);
                });
            }

            public void Succeed()
            {
                if (Failures > 0)
                {
                    _locker.SafeWrite(() =>
                    {
                        _failures = 0;
                        _available = DateTime.MinValue;
                    });
                }
            }

            private readonly ReaderWriterLockSlim _locker = new();
        }

        private readonly ReaderWriterLockSlim _locker = new();

        #endregion

        #region " Requests "

        private ILog Log { get; }

        public async Task<bool> ProcessApi(Func<NodeApiClient, Task> action)
        {
            var (endpoint, status) = GetEndpoint(_api);
            try
            {
                await action(BuildApiClient(endpoint));
                status.Succeed();
                return true;
            }
            catch (Exception ex)
            {
                status.Fail();
                if (ex is ApiException er && er.Error != null)
                {
                    await Log.Error(er, error: JObject.FromObject(er.Error).ToString(), context: endpoint);
                }
                else
                {
                    await Log.Error(ex, context: endpoint);
                }
                return false;
            }
        }

        public async Task<bool> ProcessHistory(Func<HttpClient, Task> action)
        {
            var (endpoint, status) = GetEndpoint(_history);
            try
            {
                await action(BuildHttpClient(endpoint));
                status.Succeed();
                return true;
            }
            catch (Exception ex)
            {
                status.Fail();
                await Log.Error(ex, context: endpoint);
                return false;
            }
        }

        private static readonly FieldInfo HttpClientField =
            typeof(NodeApiClient).GetField("httpClient", BindingFlags.Instance | BindingFlags.NonPublic);
        private static readonly FieldInfo HandlerField =
            typeof(HttpMessageInvoker).GetField("_handler", BindingFlags.Instance | BindingFlags.NonPublic);
        private NodeApiClient BuildApiClient(string endpoint)
        {
            var client = new NodeApiClient(endpoint);
            var httpClient = (HttpClient)HttpClientField.GetValue(client);
            httpClient.Timeout = QuickTimeout;
            var handler = (HttpClientHandler)HandlerField.GetValue(httpClient);
            HandlerField.SetValue(httpClient, new MessageHandler(handler, Log));
            return client;
        }

        private HttpClient BuildHttpClient(string endpoint)
        {
            var handler = new MessageHandler(new HttpClientHandler(), Log);
            return new HttpClient(handler) { BaseAddress = new Uri(endpoint), Timeout = QuickTimeout };
        }

        #endregion

    }
}
