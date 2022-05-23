using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;
using WaxRentals.Data.Entities;
using WaxRentals.Data.Manager;
using WaxRentals.Monitoring.Logging;
using WaxRentals.Monitoring.Prices;
using WaxRentals.Waxp.Transact;
using static WaxRentals.Monitoring.Config.Constants;
using static WaxRentals.Waxp.Config.Constants;

namespace WaxRentals.Processing.Processors
{
    internal class TrackWaxProcessor : Processor<IEnumerable<Transfer>>
    {

        protected override bool ProcessMultiplePerTick => false;

        private IClientFactory Client { get; }
        private IPriceMonitor Prices { get; }

        public TrackWaxProcessor(IDataFactory factory, IClientFactory client, IPriceMonitor prices)
            : base(factory)
        {
            Client = client;
            Prices = prices;
        }

        protected override Func<Task<IEnumerable<Transfer>>> Get => PullHistory;

        internal async Task<IEnumerable<Transfer>> PullHistory()
        {
            List<TransferBlock> blocks = new();
            var success = await Client.ProcessHistory(async client =>
            {
                var last = Factory.TrackWax.GetLastHistoryCheck()?.AddMilliseconds(1);
                var history = await client.GetStringAsync(Protocol.HistoryBasePath + last?.ToString("s"));

                foreach (var block in JObject.Parse(history).SelectTokens(Protocol.TransferBlocks))
                {
                    // Can't do this in a Select for some reason.
                    // Error: The expression cannot be evaluated.  A common cause of this error is attempting to pass a lambda into a delegate.
                    blocks.Add(block.ToObject<TransferBlock>());
                }
            });

            if (success)
            {
                var result = blocks.Select(Map);
                Factory.TrackWax.SetLastHistoryCheck(DateTime.UtcNow);
                return result;
            }
            return Enumerable.Empty<Transfer>();
        }

        protected async override Task Process(IEnumerable<Transfer> transfers)
        {
            foreach (var transfer in transfers)
            {
                var address = IsBananoAddress(transfer.Memo) ? transfer.Memo : null;
                var skip = address == null || transfer.Amount < Protocol.MinimumTransaction;
                var banano = transfer.Amount * SafeDivide(Prices.Wax, Prices.Banano);
                if (await Factory.Insert.OpenPurchase(transfer.Amount, transfer.Hash, address, banano, skip ? Status.Processed : Status.New))
                {
                    Tracker.Track("Received WAX", transfer.Amount, Coins.Wax, earned: transfer.Amount * Prices.Wax);
                }
            }
        }

        private static Transfer Map(TransferBlock block)
        {
            return new Transfer
            {
                Hash = block.transaction_id,
                Amount = block.data.amount,
                Memo = block.data.memo
            };
        }

        private static bool IsBananoAddress(string memo)
        {
            return Regex.IsMatch(memo ?? "", Protocol.BananoAddressRegex, RegexOptions.IgnoreCase);
        }

        private decimal SafeDivide(decimal numerator, decimal denominator)
        {
            if (numerator == 0 || denominator == 0)
            {
                return 0;
            }
            return numerator / denominator;
        }

    }

    internal class Transfer
    {

        public decimal Amount { get; set; }
        public string Memo { get; set; }
        public string Hash { get; set; }

    }
    internal class TransferBlock
    {
        public string transaction_id { get; set; }
        public TransferAction data { get; set; }
    }

    internal class TransferAction
    {
        public decimal amount { get; set; }
        public string memo { get; set; }
    }

}
