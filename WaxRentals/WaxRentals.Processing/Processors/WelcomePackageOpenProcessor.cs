using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WaxRentals.Service.Shared.Connectors;
using WaxRentals.Service.Shared.Entities;

namespace WaxRentals.Processing.Processors
{
    internal class WelcomePackageOpenProcessor : Processor<Result<IEnumerable<WelcomePackageInfo>>>
    {

        private IWelcomePackageService Packages { get; }
        private IBananoService Banano { get; }

        public WelcomePackageOpenProcessor(ITrackService track, IWelcomePackageService packages, IBananoService banano)
            : base(track)
        {
            Packages = packages;
            Banano = banano;
        }

        protected override Func<Task<Result<IEnumerable<WelcomePackageInfo>>>> Get => Packages.New;
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
                var result = await Banano.WelcomeAccountBalance(package.Id);
                if (result.Success && result.Value >= package.Banano)
                {
                    await Packages.ProcessPayment(package.Id);
                    Notify($"Received welcome package payment for {package.Memo}.");
                }
            }
            catch (Exception ex)
            {
                Log(ex, context: package);
            }
        }

    }
}
