using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WaxRentals.Banano.Transact;
using WaxRentals.Service.Shared.Connectors;
using WaxRentals.Service.Shared.Entities;

namespace WaxRentals.Processing.Processors
{
    internal class WelcomePackageOpenProcessor : Processor<Result<IEnumerable<WelcomePackageInfo>>>
    {

        protected override bool ProcessMultiplePerTick => false;

        private IWelcomePackageService Packages { get; }
        private IBananoAccountFactory Banano { get; }

        public WelcomePackageOpenProcessor(ITrackService track, IWelcomePackageService packages, IBananoAccountFactory banano)
            : base(track)
        {
            Packages = packages;
            Banano = banano;
        }

        protected override Func<Task<Result<IEnumerable<WelcomePackageInfo>>>> Get => Packages.New;
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
                var account = Banano.BuildWelcomeAccount(package.Id);
                var balance = await account.GetBalance();
                if (balance >= package.Banano)
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
