using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WaxRentals.Banano.Transact;
using WaxRentals.Data.Entities;
using WaxRentals.Data.Manager;
using static WaxRentals.Banano.Config.Constants;

namespace WaxRentals.Processing.Processors
{
    internal class WelcomePackageSweepProcessor : Processor<IEnumerable<WelcomePackage>>
    {

        protected override bool ProcessMultiplePerTick => false;

        private IBananoAccountFactory Banano { get; }

        public WelcomePackageSweepProcessor(IDataFactory factory, IBananoAccountFactory banano)
            : base(factory)
        {
            Banano = banano;
        }

        protected override Func<Task<IEnumerable<WelcomePackage>>> Get => Factory.Process.PullSweepableWelcomePackages;
        protected async override Task Process(IEnumerable<WelcomePackage> packages)
        {
            var tasks = packages.Select(Process);
            await Task.WhenAll(tasks);
        }

        private async Task Process(WelcomePackage package)
        {
            try
            {
                var account = Banano.BuildWelcomeAccount((uint)package.PackageId);
                var amount = await account.GetBalance();
                var hash = await account.Send(Protocol.Address, amount);
                await Factory.Process.ProcessWelcomePackageSweep(package.PackageId, hash);
            }
            catch (Exception ex)
            {
                await Factory.Log.Error(ex, context: package);
            }
        }

    }
}
