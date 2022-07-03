using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;
using WaxRentals.Processing.Extensions;
using WaxRentals.Service.Shared.Connectors;
using WaxRentals.Service.Shared.Entities;
using WaxRentals.Waxp;
using WaxRentals.Waxp.Transact;
using static WaxRentals.Monitoring.Config.Constants;
using static WaxRentals.Waxp.Config.Constants;

namespace WaxRentals.Processing.Processors
{
    internal class WelcomePackageNftProcessor : Processor<Result<IEnumerable<WelcomePackageInfo>>>
    {

        protected override bool ProcessMultiplePerTick => false;

        private IWelcomePackageService Packages { get; }
        private IWaxAccounts Wax { get; }


        public WelcomePackageNftProcessor(ITrackService track, IWelcomePackageService packages, IWaxAccounts wax)
            : base(track)
        {
            Packages = packages;
            Wax = wax;
        }

        protected override Func<Task<Result<IEnumerable<WelcomePackageInfo>>>> Get => Packages.MissingNfts;
        protected async override Task Process(Result<IEnumerable<WelcomePackageInfo>> result)
        {
            if (result.Success)
            {
                var nfts = await GetNfts();
                var bag = new ConcurrentBag<Nft>(nfts);
                var tasks = result.Value.Select(package => Process(package, bag));
                await Task.WhenAll(tasks);
            }
        }

        private async Task Process(WelcomePackageInfo package, ConcurrentBag<Nft> nfts)
        {
            try
            {
                if (nfts.TryTake(out Nft starter))
                {
                    var (success, hash) = await SendNft(package.MemoToAccount(), starter);
                    if (success)
                    {
                        await Packages.ProcessNft(package.Id, hash);
                    }
                }
            }
            catch (Exception ex)
            {
                Log(ex, context: package);
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
                Log(ex, context: account);
                return (false, null);
            }
        }

        private async Task<IEnumerable<Nft>> GetNfts()
        {
            try
            {
                var random = new Random();
                var data = await new QuickTimeoutWebClient().DownloadStringTaskAsync(Locations.Assets, QuickTimeout);
                var json = JObject.Parse(data);
                return json.SelectTokens(Protocol.Assets)
                           .Select(token => token.ToObject<Nft>())
                           .OrderBy(nft => random.Next()) // Randomize for better distribution distribution.
                           .ToList();
            }
            catch (Exception ex)
            {
                Log(ex, context: Locations.Assets);
                return Enumerable.Empty<Nft>();
            }
        }

    }
}
