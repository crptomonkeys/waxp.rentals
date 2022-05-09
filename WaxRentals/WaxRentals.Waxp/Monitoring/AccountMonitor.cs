using System;
using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json.Linq;
using WaxRentals.Monitoring;
using WaxRentals.Waxp.Transact;
using static WaxRentals.Waxp.Config.Constants;

namespace WaxRentals.Waxp.Monitoring
{
    internal class AccountMonitor : Monitor<IEnumerable<(decimal, string)>>
    {

        private readonly string _account;
        private readonly ClientFactory _client;
        private DateTime? _last;

        public AccountMonitor(TimeSpan interval, string account, ClientFactory client)
            : base(interval)
        {
            _account = account;
            _client = client;
        }
        
        protected override bool Tick(out IEnumerable<(decimal, string)> result)
        {
            List<TransferAction> actions = new List<TransferAction>();
            var success = _client.ProcessHistory(client =>
            {
                var history = client.GetStringAsync(Protocol.HistoryBasePath + _last?.ToString("s")).GetAwaiter().GetResult();
                foreach (var action in JObject.Parse(history).SelectTokens(Protocol.TransferActions))
                {
                    // Can't do this in a Select for some reason.
                    // Error: The expression cannot be evaluated.  A common cause of this error is attempting to pass a lambda into a delegate.
                    actions.Add(action.ToObject<TransferAction>());
                }
                _last = DateTime.UtcNow; // We'll definitely want to track this separately so that restarting doesn't reprocess all transactions!
            });
            result = success ? actions.Select(action => (action.amount, action.memo)) : Enumerable.Empty<(decimal, string)>();
            return success;
        }

        private class TransferAction
        {
            public decimal amount { get; set; }
            public string memo { get; set; }
        }

    }
}
