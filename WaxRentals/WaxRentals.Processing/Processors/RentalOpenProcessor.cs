using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WaxRentals.Banano.Transact;
using WaxRentals.Data.Entities;
using WaxRentals.Data.Manager;

namespace WaxRentals.Processing.Processors
{
    internal class RentalOpenProcessor : Processor<IEnumerable<Rental>>
    {

        protected override bool ProcessMultiplePerTick => false;

        private IBananoAccountFactory Banano { get; }

        public RentalOpenProcessor(IDataFactory factory, IBananoAccountFactory banano)
            : base(factory)
        {
            Banano = banano;
        }

        protected override Func<Task<IEnumerable<Rental>>> Get => Factory.Process.PullNewRentals;
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
                var balance = await account.GetBalance();
                if (balance >= rental.Banano)
                {
                    await Factory.Process.ProcessRentalPayment(rental.RentalId);
                }
            }
            catch (Exception ex)
            {
                await Factory.Log.Error(ex, context: rental);
            }
        }

    }
}
