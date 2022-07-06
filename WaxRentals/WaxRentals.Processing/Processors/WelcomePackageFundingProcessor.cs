using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WaxRentals.Service.Shared.Connectors;
using WaxRentals.Service.Shared.Entities;
using static WaxRentals.Service.Shared.Config.Constants;

namespace WaxRentals.Processing.Processors
{
    internal class WelcomePackageFundingProcessor : Processor<Result<IEnumerable<WelcomePackageInfo>>>
    {

        private IWelcomePackageService Packages { get; }
        private IWaxService Wax { get; }
        private IAppService App { get; }
        
        public WelcomePackageFundingProcessor(ITrackService track, IWelcomePackageService packages, IWaxService wax, IAppService app)
            : base(track)
        {
            Packages = packages;
            Wax = wax;
            App = app;
        }

        protected override Func<Task<Result<IEnumerable<WelcomePackageInfo>>>> Get => Packages.Paid;
        protected async override Task<bool> Process(Result<IEnumerable<WelcomePackageInfo>> result)
        {
            if (result.Success && result.Value != null)
            {
                var tasks = result.Value.Select(Process);
                await Task.WhenAll(tasks);
            }
            return false;
        }

        private async Task Process(WelcomePackageInfo package)
        {
            try
            {
                var result = await Wax.Send(package.WaxAccount, package.Wax, memo: package.Memo);
                if (result.Success)
                {
                    var task = Packages.ProcessFunding(package.Id, result.Value);
                    LogTransaction("Sent WAX", package.Wax, Coins.Wax, spent: await ToUsd(package.Wax));
                    await task;
                }
            }
            catch (Exception ex)
            {
                Log(ex, context: package);
            }
        }

        private async Task<decimal> ToUsd(decimal wax)
        {
            var result = await App.State();
            return wax * (result.Value?.WaxPrice ?? 0);
        }

    }
}
