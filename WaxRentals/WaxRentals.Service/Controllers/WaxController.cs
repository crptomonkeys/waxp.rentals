using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json.Linq;
using WaxRentals.Data.Manager;
using WaxRentals.Service.Caching;
using WaxRentals.Service.Config;
using WaxRentals.Waxp;
using WaxRentals.Waxp.History;
using WaxRentals.Waxp.Transact;
using static WaxRentals.Monitoring.Config.Constants;
using static WaxRentals.Waxp.Config.Constants;
using Entities = WaxRentals.Service.Shared.Entities;

#nullable disable

namespace WaxRentals.Service.Controllers
{
    public class WaxController : ServiceBase
    {

        private IWaxAccounts Wax { get; }
        private IWaxHistoryChecker History { get; }
        private WaxInfoCache WaxInfo { get; }
        private Mapper Mapper { get; }

        public WaxController(
            ILog log,
            IWaxAccounts wax,
            IWaxHistoryChecker history,
            WaxInfoCache waxInfo,
            Mapper mapper)
            : base(log)
        {
            Wax = wax;
            History = history;
            WaxInfo = waxInfo;
            Mapper = mapper;
        }

        [HttpGet("Nfts")]
        public async Task<JsonResult> Nfts()
        {
            try
            {
                var random = new Random();
                var data = await new QuickTimeoutWebClient().DownloadStringTaskAsync(string.Format(Locations.Assets, Wax.Primary.Account), QuickTimeout);
                var json = JObject.Parse(data);
                return Succeed(json.SelectTokens(Protocol.Assets)
                                   .Select(token => token.ToObject<Nft>())
                                   .Select(nft => new Entities.Nft { AssetId = nft.AssetId })
                                   .OrderBy(nft => random.Next()) // Randomize for better distribution distribution.
                                   .ToList());
            }
            catch (Exception ex)
            {
                await Log.Error(ex, context: string.Format(Locations.Assets, Wax.Primary.Account));
                return Fail("Unable to pull NFTs list.");
            }
        }

        [HttpGet("LatestTransfers")]
        public async Task<JsonResult> LatestTransfers()
        {
            var transfers = await History.PullLatestHistory();
            return Succeed(transfers.Select(Mapper.Map));
        }

        [HttpPost("Sweep")]
        public async Task<JsonResult> Sweep()
        {
            var primary = WaxInfo.GetBalances(Wax.Primary.Account);
            var yesterday = WaxInfo.GetBalances(Wax.Yesterday.Account);
            var today = WaxInfo.GetBalances(Wax.Today.Account);

            var tasks = new List<Task>();
            if (primary?.Available > 0)
            {
                tasks.Add(Wax.Primary.Send(Wax.Today.Account, primary.Available));
            }
            if (yesterday?.Available > 0)
            {
                tasks.Add(Wax.Yesterday.Send(Wax.Today.Account, yesterday.Available));
            }
            if (today?.Unstaking > 0)
            {
                tasks.Add(Wax.Today.ClaimRefund());
            }

            if (tasks.Any())
            {
                await Task.WhenAll(tasks);
                await WaxInfo.Invalidate();
            }
            return Succeed();
        }

        [HttpPost("Stake")]
        public async Task<JsonResult> Stake([FromBody] Entities.Input.StakeInput input)
        {
            // Fund the source account.
            var source = input.Source == null ? Wax.GetAccount(input.Days.Value) : Wax.GetAccount(input.Source);
            var needed = input.Cpu + input.Net;
            var (sourceSuccess, sourceBalances) = await source.GetBalances();
            if (sourceSuccess && sourceBalances.Available < needed)
            {
                var (success, _) = await Wax.Today.Send(source.Account, needed - sourceBalances.Available);
                if (!success)
                {
                    return Fail($"Failed to transfer {needed} {Coins.Wax} to from {Wax.Today.Account} to {source.Account} to support stake.");
                }
            }

            var (stakeSuccess, hash) = await source.Stake(input.Target, input.Cpu, input.Net);
            if (stakeSuccess)
            {
                return Succeed(new Entities.NewStakeInfo { SourceAccount = source.Account, Transaction = hash });
            }
            return Fail($"Failed to stake {input.Cpu + input.Net} {Coins.Wax} from {source.Account} to {input.Target}.");
        }

        [HttpPost("Unstake")]
        public async Task<JsonResult> Unstake([FromBody] Entities.Input.StakeInput input)
        {
            var wax = Wax.GetAccount(input.Source);
            var (success, hash) = await wax.Unstake(input.Target, input.Cpu, input.Net);
            return success ? Succeed(hash) : Fail("Unstaking unsuccessful.");
        }


        [HttpPost("Send")]
        public async Task<JsonResult> Send([FromBody] Entities.Input.SendWaxInput input)
        {
            var account = input.Source == null ? Wax.Today : Wax.GetAccount(input.Source);
            var result = await account.GetBalances();
            if (result.Success)
            {
                var balances = result.Balances;
                if (balances.Available >= input.Amount)
                {
                    var (success, hash) = await account.Send(input.Recipient, input.Amount, input.Memo);
                    return success ? Succeed(hash) : Fail($"Failed to send {Coins.Wax} from {input.Source} to {input.Recipient}.");
                }
                else
                {
                    return Fail($"Requested {input.Amount} {Coins.Wax} but only have {balances.Available} {Coins.Wax} available.");
                }
            }
            return Fail($"Unable to retrieve balances for {input.Source}.");
        }

        [HttpPost("SendAsset")]
        public async Task<JsonResult> SendAsset([FromBody] Entities.Input.SendNftInput input)
        {
            try
            {
                var (success, transaction) = await Wax.Primary.SendAsset(input.Recipient, input.AssetId, input.Memo);
                return success ? Succeed(transaction) : Fail($"Failed to send NFT to {input.Recipient}.");
            }
            catch (Exception ex)
            {
                await Log.Error(ex, context: input.Recipient);
                return Fail($"Failed to send NFT to {input.Recipient}.");
            }
        }

    }
}
