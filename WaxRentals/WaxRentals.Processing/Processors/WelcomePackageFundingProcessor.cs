using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;
using WaxRentals.Data.Entities;
using WaxRentals.Data.Manager;
using WaxRentals.Monitoring.Notifications;
using WaxRentals.Monitoring.Prices;
using WaxRentals.Processing.Tracking;
using WaxRentals.Waxp;
using WaxRentals.Waxp.Transact;
using static WaxRentals.Monitoring.Config.Constants;
using static WaxRentals.Waxp.Config.Constants;

namespace WaxRentals.Processing.Processors
{
    internal class WelcomePackageFundingProcessor : Processor<IEnumerable<WelcomePackage>>
    {

        protected override bool ProcessMultiplePerTick => false;

        private IWaxAccounts Wax { get; }
        private IPriceMonitor Prices { get; }
        private ITracker Tracker { get; }
        private ITelegramNotifier Telegram { get; }


        public WelcomePackageFundingProcessor(IDataFactory factory, IWaxAccounts wax, IPriceMonitor prices, ITracker tracker, ITelegramNotifier telegram)
            : base(factory)
        {
            Wax = wax;
            Prices = prices;
            Tracker = tracker;
            Telegram = telegram;
        }

        protected override Func<Task<IEnumerable<WelcomePackage>>> Get => Factory.Process.PullPaidWelcomePackagesToFund;
        protected async override Task Process(IEnumerable<WelcomePackage> packages)
        {
            var nfts = await GetNfts();
            var bag = new ConcurrentBag<Nft>(nfts);
            var tasks = packages.Select(package => Process(package, bag));
            await Task.WhenAll(tasks);
        }

        private async Task Process(WelcomePackage package, ConcurrentBag<Nft> nfts)
        {
            try
            {
                var balance = (await Wax.Today.GetBalances()).Available;
                if (balance > package.Wax)
                {
                    var (success, fund) = await Wax.Today.Send(package.TargetWaxAccount, package.Wax, package.Memo);
                    if (success)
                    {
                        string nft = null;
                        if (nfts.TryTake(out Nft starter))
                        {
                            nft = await SendNft(package.Memo, starter);
                        }
                        var task = Factory.Process.ProcessWelcomePackageFunding(package.PackageId, fund, nft);
                        Tracker.Track("Sent WAX", package.Wax, Coins.Wax, spent: package.Wax * Prices.Wax);
                        Telegram.Send($"Received welcome package payment for {package.Memo}.");
                        await task;
                    }
                }
            }
            catch (Exception ex)
            {
                await Factory.Log.Error(ex, context: package);
            }
        }

        private async Task<string> SendNft(string memo, Nft nft)
        {
            try
            {
                var account = memo.Replace("DOT", ".", StringComparison.Ordinal);
                var (_, hash) = await Wax.Primary.SendAsset(account, nft.AssetId, "Welcome!");
                return hash;
            }
            catch (Exception ex)
            {
                await Factory.Log.Error(ex, context: memo);
                return null;
            }
        }

        private async Task<IEnumerable<Nft>> GetNfts()
        {
            try
            {
                var random = new Random();
                var json = JObject.Parse(new QuickTimeoutWebClient().DownloadString(Locations.Assets, TimeSpan.FromSeconds(5)));
                return json.SelectTokens(Protocol.Assets)
                           .Select(token => token.ToObject<Nft>())
                           .OrderBy(nft => random.Next()) // Randomize for better distribution distribution.
                           .ToList();
            }
            catch (Exception ex)
            {
                await Factory.Log.Error(ex, context: Locations.Assets);
                return Enumerable.Empty<Nft>();
            }
        }

    }
}
