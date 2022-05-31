using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WaxRentals.Data.Entities;
using WaxRentals.Data.Manager;
using WaxRentals.Monitoring.Notifications;
using WaxRentals.Monitoring.Prices;
using WaxRentals.Processing.Tracking;
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
                var balance = (await Wax.Today.GetBalances()).Available;
                if (balance > package.Wax)
                {
                    var (success, fund) = await Wax.Today.Send(
                        package.TargetWaxAccount,
                        package.Wax,
                        $"{package.Memo}{Protocol.NewUser.MemoRefundOnExists}");
                    if (success)
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
