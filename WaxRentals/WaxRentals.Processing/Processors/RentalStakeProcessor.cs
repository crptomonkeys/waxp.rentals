using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WaxRentals.Service.Shared.Connectors;
using WaxRentals.Service.Shared.Entities;
using WaxRentals.Waxp.Transact;

namespace WaxRentals.Processing.Processors
{
    internal class RentalStakeProcessor : Processor<Result<IEnumerable<RentalInfo>>>
    {

        protected override bool ProcessMultiplePerTick => false;

        private IRentalService Rentals { get; }
        private IWaxAccounts Wax { get; }
        
        public RentalStakeProcessor(ITrackService track, IRentalService rentals, IWaxAccounts wax)
            : base(track)
        {
            Rentals = rentals;
            Wax = wax;
        }

        protected override Func<Task<Result<IEnumerable<RentalInfo>>>> Get => Rentals.Paid;
        protected async override Task Process(Result<IEnumerable<RentalInfo>> result)
        {
            if (result.Success)
            {
                var tasks = result.Value.Select(Process);
                await Task.WhenAll(tasks);
            }
        }

        private async Task Process(RentalInfo rental)
        {
            try
            {
                // Fund the source account.
                var source = Wax.GetAccount(rental.Days);
                var needed = rental.Cpu + rental.Net;
                var (sourceSuccess, sourceBalances) = await source.GetBalances();
                if (sourceBalances.Available < needed)
                {
                    await Wax.Today.Send(source.Account, needed - sourceBalances.Available);
                }

                var (stakeSuccess, hash) = await source.Stake(rental.WaxAccount, rental.Cpu, rental.Net);
                if (stakeSuccess)
                {
                    await Rentals.ProcessStake(rental.Id, source.Account, hash);
                }
            }
            catch (Exception ex)
            {
                Log(ex, context: rental);
            }
        }

    }
}
