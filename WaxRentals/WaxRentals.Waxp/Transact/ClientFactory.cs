using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using Eos.Api;
using WaxRentals.Data.Manager;
using WaxRentals.Monitoring.Extensions;
using WaxRentals.Waxp.Monitoring;
using static WaxRentals.Monitoring.Config.Constants;

namespace WaxRentals.Waxp.Transact
{
    internal class ClientFactory
    {

        #region " Endpoints "

        private readonly IDictionary<string, Status> _api = new Dictionary<string, Status>();
        private readonly IDictionary<string, Status> _history = new Dictionary<string, Status>();
        private readonly Random _random = new Random();

        public ClientFactory(EndpointMonitor monitor, ILog log)
        {
            Log = log;

            monitor.Updated += Monitor_Updated;
            monitor.Initialize();
        }

        private void Monitor_Updated(object sender, EndpointMonitor.Endpoints e)
        {
            Update(_api, e.Api);
            Update(_history, e.History);
        }

        private void Update(IDictionary<string, Status> endpoints, IEnumerable<string> found)
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

            private readonly ReaderWriterLockSlim _locker = new ReaderWriterLockSlim();
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
                await action(new NodeApiClient(endpoint));
                status.Succeed();
                return true;
            }
            catch (Exception ex) when (!(ex is ApiException))
            {
                status.Fail();
                await Log.Error(ex);
                return false;
            }
        }

        public bool ProcessHistory(Action<HttpClient> action)
        {
            var (endpoint, status) = GetEndpoint(_history);
            try
            {
                action(new HttpClient() { BaseAddress = new Uri(endpoint) });
                status.Succeed();
                return true;
            }
            catch (Exception ex)
            {
                status.Fail();
                Log.Error(ex);
                return false;
            }
        }

        #endregion

    }
}
