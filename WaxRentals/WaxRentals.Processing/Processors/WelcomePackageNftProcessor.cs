using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;
using WaxRentals.Data.Entities;
using WaxRentals.Data.Manager;
using WaxRentals.Processing.Extensions;
using WaxRentals.Waxp;
using WaxRentals.Waxp.Transact;
using static WaxRentals.Monitoring.Config.Constants;
using static WaxRentals.Waxp.Config.Constants;

namespace WaxRentals.Processing.Processors
{
    internal class WelcomePackageNftProcessor : Processor<IEnumerable<WelcomePackage>>
    {

        protected override bool ProcessMultiplePerTick => false;

        private IWaxAccounts Wax { get; }


        public WelcomePackageNftProcessor(IDataFactory factory, IWaxAccounts wax)
            : base(factory)
        {
            Wax = wax;
        }

        protected override Func<Task<IEnumerable<WelcomePackage>>> Get => Factory.Process.PullFundedWelcomePackagesMissingNft;
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
                if (nfts.TryTake(out Nft starter))
                {
                    var (success, hash) = await SendNft(package.MemoToAccount(), starter);
                    if (success)
                    {
                        await Factory.Process.ProcessWelcomePackageNft(package.PackageId, hash);
                    }
                }
            }
            catch (Exception ex)
            {
                await Factory.Log.Error(ex, context: package);
            }
        }

        private async Task<(bool, string)> SendNft(string account, Nft nft)
        {
            try
            {
                return await Wax.Primary.SendAsset(account, nft.AssetId, "Welcome!");
            }
            catch (Exception ex)
            {
                await Factory.Log.Error(ex, context: account);
                return (false, null);
            }
        }

        private async Task<IEnumerable<Nft>> GetNfts()
        {
            try
            {
                var random = new Random();
                var json = JObject.Parse(new QuickTimeoutWebClient().DownloadString(Locations.Assets, QuickTimeout));
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
