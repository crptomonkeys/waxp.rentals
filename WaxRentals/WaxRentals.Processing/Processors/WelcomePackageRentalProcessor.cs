using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WaxRentals.Data.Entities;
using WaxRentals.Data.Manager;
using WaxRentals.Processing.Extensions;
using static WaxRentals.Waxp.Config.Constants;

namespace WaxRentals.Processing.Processors
{
    internal class WelcomePackageRentalProcessor : Processor<IEnumerable<WelcomePackage>>
    {

        protected override bool ProcessMultiplePerTick => false;

        public WelcomePackageRentalProcessor(IDataFactory factory) : base(factory) { }

        protected override Func<Task<IEnumerable<WelcomePackage>>> Get => Factory.Process.PullFundedWelcomePackagesMissingRental;
        protected async override Task Process(IEnumerable<WelcomePackage> packages)
        {
            var tasks = packages.Select(Process);
            await Task.WhenAll(tasks);
        }

        private async Task Process(WelcomePackage package)
        {
            try
            {
                var rentalId = await Factory.Insert.OpenRental(
                    package.MemoToAccount(),
                    Protocol.NewUser.FreeRentalDays,
                    Protocol.NewUser.FreeCpu,
                    Protocol.NewUser.FreeNet,
                    0,
                    Status.Pending);
                await Factory.Process.ProcessWelcomePackageRental(package.PackageId, rentalId);
            }
            catch (Exception ex)
            {
                await Factory.Log.Error(ex, context: package);
            }
        }

    }
}
