using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WaxRentals.Service.Shared.Connectors;
using WaxRentals.Service.Shared.Entities;

namespace WaxRentals.Processing.Processors
{
    internal class WelcomePackageSweepProcessor : Processor<Result<IEnumerable<WelcomePackageInfo>>>
    {

        protected override bool ProcessMultiplePerTick => false;

        private IWelcomePackageService Packages { get; }
        private IBananoService Banano { get; }

        public WelcomePackageSweepProcessor(ITrackService track, IWelcomePackageService packages, IBananoService banano)
            : base(track)
        {
            Packages = packages;
            Banano = banano;
        }

        protected override Func<Task<Result<IEnumerable<WelcomePackageInfo>>>> Get => Packages.Sweepable;
        protected async override Task Process(Result<IEnumerable<WelcomePackageInfo>> result)
        {
            if (result.Success && result.Value != null)
            {
                var tasks = result.Value.Select(Process);
                await Task.WhenAll(tasks);
            }
        }

        private async Task Process(WelcomePackageInfo package)
        {
            try
            {
                var result = await Banano.SweepWelcomeAccount(package.Id);
                if (result.Success)
                {
                    await Packages.ProcessSweep(package.Id, result.Value);
                }
            }
            catch (Exception ex)
            {
                Log(ex, context: package);
            }
        }

    }
}
