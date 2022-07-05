using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;
using WaxRentals.Data.Manager;
using WaxRentals.Service.Shared.Config;
using WaxRentals.Waxp.Transact;
using static WaxRentals.Waxp.Config.Constants;

namespace WaxRentals.Waxp.History
{
    public interface IWaxHistoryChecker
    {
        Task<IEnumerable<TransferInfo>> PullLatestHistory();
    }

    internal class WaxHistoryChecker : IWaxHistoryChecker
    {

        private AccountNames Names { get; }
        private IDataFactory Data { get; }
        private IClientFactory Client { get; }

        public WaxHistoryChecker(AccountNames names, IDataFactory data, IClientFactory client)
        {
            Names = names;
            Data = data;
            Client = client;
        }

        public async Task<IEnumerable<TransferInfo>> PullLatestHistory()
        {
            List<TransferBlock> blocks = new();
            var success = await Client.ProcessHistory(async client =>
            {
                var last = (await Data.TrackWax.GetLastHistoryCheck())?.AddMilliseconds(1);
                var history = await client.GetStringAsync(string.Format(Protocol.HistoryBasePath, Names.Primary) + last?.ToString("s"));

                foreach (var block in JObject.Parse(history).SelectTokens(string.Format(Protocol.TransferBlocks, Names.Primary)))
                {
                    // Can't do this in a Select for some reason.
                    // Error: The expression cannot be evaluated.  A common cause of this error is attempting to pass a lambda into a delegate.
                    blocks.Add(block.ToObject<TransferBlock>());
                }
            });

            if (success)
            {
                var result = blocks.Select(Map);
                await Data.TrackWax.SetLastHistoryCheck(DateTime.UtcNow);
                return result.Select(Map);
            }
            return Enumerable.Empty<TransferInfo>();
        }

        #region " Static Helper Methods "

        private static TransferInfo Map(Transfer transfer)
        {
            var address = IsBananoAddress(transfer.Memo) ? transfer.Memo : null;
            var skip = address == null || transfer.Amount < Protocol.MinimumTransaction;
            return new TransferInfo
            {
                Source = transfer.From,
                Transaction = transfer.Hash,
                Amount = transfer.Amount,
                BananoPaymentAddress = address,
                SkipPayment = skip
            };
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

        private static bool IsBananoAddress(string memo)
        {
            return Regex.IsMatch(memo ?? "", Constants.Banano.Protocol.AddressRegex, RegexOptions.IgnoreCase);
        }

        #endregion

        #region " Intermediate Classes "

        internal class Transfer
        {
            public decimal Amount { get; set; }
            public string From { get; set; }
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
            public string from { get; set; }
            public string memo { get; set; }
        }

        #endregion

    }
}
