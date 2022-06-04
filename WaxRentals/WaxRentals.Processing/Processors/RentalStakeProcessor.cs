using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WaxRentals.Data.Entities;
using WaxRentals.Data.Manager;
using WaxRentals.Waxp.Transact;

namespace WaxRentals.Processing.Processors
{
    internal class RentalStakeProcessor : Processor<IEnumerable<Rental>>
    {

        protected override bool ProcessMultiplePerTick => false;

        private IWaxAccounts Wax { get; }
        
        public RentalStakeProcessor(IDataFactory factory, IWaxAccounts wax)
            : base(factory)
        {
            Wax = wax;
        }

        protected override Func<Task<IEnumerable<Rental>>> Get => Factory.Process.PullPaidRentalsToStake;
        protected async override Task Process(IEnumerable<Rental> rentals)
        {
            var tasks = rentals.Select(Process);
            await Task.WhenAll(tasks);
        }

        private async Task Process(Rental rental)
        {
            try
            {
                // Fund the source account.
                var source = Wax.GetAccount(rental.RentalDays);
                var needed = rental.CPU + rental.NET;
                var (sourceSuccess, sourceBalances) = await source.GetBalances();
                if (sourceBalances.Available < needed)
                {
                    await Wax.Today.Send(source.Account, needed - sourceBalances.Available);
                }

                var (stakeSuccess, hash) = await source.Stake(rental.TargetWaxAccount, rental.CPU, rental.NET);
                if (stakeSuccess)
                {
                    await Factory.Process.ProcessRentalStaking(rental.RentalId, source.Account, hash);
                }
            }
            catch (Exception ex)
            {
                await Factory.Log.Error(ex, context: rental);
            }
        }

    }
}
