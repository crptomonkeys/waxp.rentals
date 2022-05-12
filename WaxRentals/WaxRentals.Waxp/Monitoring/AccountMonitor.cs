using System;
using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json.Linq;
using WaxRentals.Data.Manager;
using WaxRentals.Monitoring;
using WaxRentals.Waxp.Transact;
using static WaxRentals.Waxp.Config.Constants;

namespace WaxRentals.Waxp.Monitoring
{
    internal class AccountMonitor : Monitor<IEnumerable<Transfer>>
    {

        private readonly string _account;
        private readonly ClientFactory _client;
        private ITrackWax _wax;

        public AccountMonitor(TimeSpan interval, string account, ClientFactory client, ITrackWax wax)
            : base(interval)
        {
            _account = account;
            _client = client;
            _wax = wax;
        }
        
        protected override bool Tick(out IEnumerable<Transfer> result)
        {
            List<TransferBlock> blocks = new List<TransferBlock>();
            var success = _client.ProcessHistory(client =>
            {
                var last = _wax.GetLastHistoryCheck();
                var history = client.GetStringAsync(Protocol.HistoryBasePath + last?.ToString("s")).GetAwaiter().GetResult();
                _wax.SetLastHistoryCheck(DateTime.UtcNow);
                
                foreach (var block in JObject.Parse(history).SelectTokens(Protocol.TransferBlocks))
                {
                    // Can't do this in a Select for some reason.
                    // Error: The expression cannot be evaluated.  A common cause of this error is attempting to pass a lambda into a delegate.
                    blocks.Add(block.ToObject<TransferBlock>());
                }
            });
            result = success ? blocks.Select(Map) : Enumerable.Empty<Transfer>();
            return success;
        }

        private static Transfer Map(TransferBlock block)
        {
            return new Transfer
            {
                Hash = block.transaction_id,
                From = block.data.from,
                Amount = block.data.amount,
                Memo = block.data.memo
            };
        }

        private class TransferBlock
        {
            public string transaction_id { get; set; }
            public TransferAction data { get; set; }
        }

        private class TransferAction
        {
            public string from { get; set; }
            public decimal amount { get; set; }
            public string memo { get; set; }
        }

    }
}
