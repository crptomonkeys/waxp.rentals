using System;
using System.Threading.Tasks;
using WaxRentals.Service.Shared.Connectors;
using WaxRentals.Service.Shared.Entities;

namespace WaxRentals.Processing.Processors
{
    internal class RentalClosingProcessor : Processor<Result<RentalInfo>>
    {

        private IRentalService Rentals { get; }
        private IWaxService Wax { get; }

        public RentalClosingProcessor(ITrackService track, IRentalService rentals, IWaxService wax)
            : base(track)
        {
            Rentals = rentals;
            Wax = wax;
        }

        protected override Func<Task<Result<RentalInfo>>> Get => Rentals.NextClosing;
        protected async override Task Process(Result<RentalInfo> result)
        {
            if (result.Success && result.Value != null)
            {
                await Process(result.Value);
            }
        }

        private async Task Process(RentalInfo rental)
        {
            var result = await Wax.Unstake(rental.Cpu, rental.Net, rental.WaxAccount, rental.SourceAccount);
            if (result.Success)
            {
                await Rentals.ProcessClosing(rental.Id, result.Value);
            }
        }

    }
}
