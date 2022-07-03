using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WaxRentals.Monitoring.Prices;
using WaxRentals.Service.Shared.Connectors;
using WaxRentals.Service.Shared.Entities;
using WaxRentals.Waxp.Transact;
using static WaxRentals.Service.Shared.Config.Constants;

namespace WaxRentals.Processing.Processors
{
    internal class WelcomePackageFundingProcessor : Processor<Result<IEnumerable<WelcomePackageInfo>>>
    {

        protected override bool ProcessMultiplePerTick => false;

        private IWelcomePackageService Packages { get; }
        private IWaxAccounts Wax { get; }
        private IPriceMonitor Prices { get; }
        
        public WelcomePackageFundingProcessor(ITrackService track, IWelcomePackageService packages, IWaxAccounts wax, IPriceMonitor prices)
            : base(track)
        {
            Packages = packages;
            Wax = wax;
            Prices = prices;
        }

        protected override Func<Task<Result<IEnumerable<WelcomePackageInfo>>>> Get => Packages.Paid;
        protected async override Task Process(Result<IEnumerable<WelcomePackageInfo>> result)
        {
            if (result.Success)
            {
                var tasks = result.Value.Select(Process);
                await Task.WhenAll(tasks);
            }
        }

        private async Task Process(WelcomePackageInfo package)
        {
            try
            {
                var (todaySuccess, todayBalances) = await Wax.Today.GetBalances();
                if (todaySuccess && todayBalances.Available > package.Wax)
                {
                    var (sendSuccess, fund) = await Wax.Today.Send(
                        package.WaxAccount,
                        package.Wax,
                        package.Memo);
                    if (sendSuccess)
                    {
                        var task = Packages.ProcessFunding(package.Id, fund);
                        LogTransaction("Sent WAX", package.Wax, Coins.Wax, spent: package.Wax * Prices.Wax);
                        await task;
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
