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
    internal class RentalSweepProcessor : Processor<IEnumerable<Rental>>
    {

        protected override bool ProcessMultiplePerTick => false;

        private IBananoAccountFactory Banano { get; }

        public RentalSweepProcessor(IDataFactory factory, IBananoAccountFactory banano)
            : base(factory)
        {
            Banano = banano;
        }

        protected override Func<Task<IEnumerable<Rental>>> Get => Factory.Process.PullSweepableRentals;
        protected async override Task Process(IEnumerable<Rental> rentals)
        {
            var tasks = rentals.Select(Process);
            await Task.WhenAll(tasks);
        }

        private async Task Process(Rental rental)
        {
            try
            {
                var account = Banano.BuildAccount((uint)rental.RentalId);
                var amount = await account.GetBalance();
                amount *= 1 / Math.Pow(10, Protocol.Decimals);
                var hash = await account.Send(Protocol.Address, amount);
                await Factory.Process.ProcessRentalSweep(rental.RentalId, hash);
            }
            catch (Exception ex)
            {
                await Factory.Log.Error(ex, context: rental);
            }
        }

    }
}
