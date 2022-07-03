using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WaxRentals.Processing.Extensions;
using WaxRentals.Service.Shared.Connectors;
using WaxRentals.Service.Shared.Entities;

namespace WaxRentals.Processing.Processors
{
    internal class WelcomePackageNftProcessor : Processor<Result<IEnumerable<WelcomePackageInfo>>>
    {

        protected override bool ProcessMultiplePerTick => false;

        private IWelcomePackageService Packages { get; }
        private IWaxService Wax { get; }


        public WelcomePackageNftProcessor(ITrackService track, IWelcomePackageService packages, IWaxService wax)
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
                await Process(result.Value);
            }
        }

        private async Task Process(IEnumerable<WelcomePackageInfo> packages)
        {
            var result = await Wax.Nfts();
            if (result.Success)
            {
                var nfts = new ConcurrentBag<Nft>(result.Value);
                var tasks = packages.Select(package => Process(package, nfts));
                await Task.WhenAll(tasks);
            }
        }

        private async Task Process(WelcomePackageInfo package, ConcurrentBag<Nft> nfts)
        {
            try
            {
                if (nfts.TryTake(out Nft starter))
                {
                    var result = await Wax.SendAsset(package.MemoToAccount(), starter.AssetId, "Welcome!");
                    if (result.Success)
                    {
                        await Packages.ProcessNft(package.Id, result.Value);
                    }
                }
            }
            catch (Exception ex)
            {
                Log(ex, context: package);
            }
        }

    }
}
