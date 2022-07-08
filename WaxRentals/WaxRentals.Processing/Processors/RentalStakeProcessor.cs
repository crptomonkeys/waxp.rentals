using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WaxRentals.Service.Shared.Connectors;
using WaxRentals.Service.Shared.Entities;

namespace WaxRentals.Processing.Processors
{
    internal class RentalStakeProcessor : Processor<Result<IEnumerable<RentalInfo>>>
    {

        private IRentalService Rentals { get; }
        private IWaxService Wax { get; }
        
        public RentalStakeProcessor(ITrackService track, IRentalService rentals, IWaxService wax)
            : base(track)
        {
            Rentals = rentals;
            Wax = wax;
        }

        protected override Func<Task<Result<IEnumerable<RentalInfo>>>> Get => Rentals.Paid;
        protected async override Task<bool> Process(Result<IEnumerable<RentalInfo>> result)
        {
            if (result.Success && result.Value != null)
            {
                var tasks = result.Value.Select(Process);
                await Task.WhenAll(tasks);
            }
            return false;
        }

        private async Task Process(RentalInfo rental)
        {
            try
            {
                var result = await Wax.Stake(rental.Cpu, rental.Net, rental.WaxAccount, days: rental.Days);
                if (result.Success)
                {
                    var info = result.Value;
                    await Rentals.ProcessStake(rental.Id, info.SourceAccount, info.Transaction);
                }
            }
            catch (Exception ex)
            {
                Log(ex, context: rental);
            }
        }

    }
}
