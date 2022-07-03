using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WaxRentals.Processing.Extensions;
using WaxRentals.Service.Shared.Connectors;
using WaxRentals.Service.Shared.Entities;
using WaxRentals.Service.Shared.Entities.Input;
using static WaxRentals.Service.Shared.Config.Constants.Wax;

namespace WaxRentals.Processing.Processors
{
    internal class WelcomePackageRentalProcessor : Processor<Result<IEnumerable<WelcomePackageInfo>>>
    {

        protected override bool ProcessMultiplePerTick => false;

        private IWelcomePackageService Packages { get; }
        private IRentalService Rentals { get; }

        public WelcomePackageRentalProcessor(ITrackService track, IWelcomePackageService packages, IRentalService rentals)
            : base(track)
        {
            Packages = packages;
            Rentals = rentals;
        }

        protected override Func<Task<Result<IEnumerable<WelcomePackageInfo>>>> Get => Packages.MissingRentals;
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
                var result = await Rentals.Create(
                    new NewRentalInput
                    {
                        Account = package.MemoToAccount(),
                        Days = NewUser.FreeRentalDays,
                        Cpu = NewUser.FreeCpu,
                        Net = NewUser.FreeNet,
                        Free = true
                    }
                );
                if (result.Success)
                {
                    await Packages.ProcessRental(package.Id, result.Value.Id);
                }
            }
            catch (Exception ex)
            {
                Log(ex, context: package);
            }
        }

    }
}
