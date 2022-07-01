using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WaxRentals.Data.Entities;
using WaxRentals.Data.Manager;
using WaxRentals.Monitoring.Prices;
using WaxRentals.Processing.Tracking;
using WaxRentals.Waxp.Transact;
using static WaxRentals.Monitoring.Config.Constants;
using static WaxRentals.Service.Shared.Config.Constants.Wax;

namespace WaxRentals.Processing.Processors
{
    internal class WelcomePackageFundingProcessor : Processor<IEnumerable<WelcomePackage>>
    {

        protected override bool ProcessMultiplePerTick => false;

        private IWaxAccounts Wax { get; }
        private IPriceMonitor Prices { get; }
        private ITracker Tracker { get; }
        
        public WelcomePackageFundingProcessor(IDataFactory factory, IWaxAccounts wax, IPriceMonitor prices, ITracker tracker)
            : base(factory)
        {
            Wax = wax;
            Prices = prices;
            Tracker = tracker;
        }

        protected override Func<Task<IEnumerable<WelcomePackage>>> Get => Factory.Process.PullPaidWelcomePackagesToFund;
        protected async override Task Process(IEnumerable<WelcomePackage> packages)
        {
            var tasks = packages.Select(Process);
            await Task.WhenAll(tasks);
        }

        private async Task Process(WelcomePackage package)
        {
            try
            {
                var (todaySuccess, todayBalances) = await Wax.Today.GetBalances();
                if (todaySuccess && todayBalances.Available > package.Wax)
                {
                    var (sendSuccess, fund) = await Wax.Today.Send(
                        package.TargetWaxAccount,
                        package.Wax,
                        $"{package.Memo}{NewUser.MemoRefundOnExists}");
                    if (sendSuccess)
                    {
                        var task = Factory.Process.ProcessWelcomePackageFunding(package.PackageId, fund);
                        Tracker.Track("Sent WAX", package.Wax, Coins.Wax, spent: package.Wax * Prices.Wax);
                        await task;
                    }
                }
            }
            catch (Exception ex)
            {
                await Factory.Log.Error(ex, context: package);
            }
        }

    }
}
