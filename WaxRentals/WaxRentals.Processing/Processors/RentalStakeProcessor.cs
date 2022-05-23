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
                var wax = rental.CPU + rental.NET;
                var balance = (await source.GetBalances()).Available;
                if (balance < wax)
                {
                    await Wax.Today.Send(source.Account, wax - balance);
                }

                var (success, hash) = await source.Stake(rental.TargetWaxAccount, rental.CPU, rental.NET);
                if (success)
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
