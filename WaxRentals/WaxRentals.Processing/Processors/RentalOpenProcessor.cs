using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WaxRentals.Banano.Transact;
using WaxRentals.Service.Shared.Connectors;
using WaxRentals.Service.Shared.Entities;

namespace WaxRentals.Processing.Processors
{
    internal class RentalOpenProcessor : Processor<Result<IEnumerable<RentalInfo>>>
    {

        protected override bool ProcessMultiplePerTick => false;

        private IRentalService Rentals { get; }
        private IBananoAccountFactory Banano { get; }

        public RentalOpenProcessor(ITrackService track, IRentalService rentals, IBananoAccountFactory banano)
            : base(track)
        {
            Rentals = rentals;
            Banano = banano;
        }

        protected override Func<Task<Result<IEnumerable<RentalInfo>>>> Get => Rentals.New;
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
                var account = Banano.BuildAccount(rental.Id);
                var balance = await account.GetBalance();
                if (balance >= rental.Banano)
                {
                    await Rentals.ProcessPayment(rental.Id);
                    Notify($"Received rental payment for {rental.WaxAccount}.");
                }
            }
            catch (Exception ex)
            {
                Log(ex, context: rental);
            }
        }

    }
}
