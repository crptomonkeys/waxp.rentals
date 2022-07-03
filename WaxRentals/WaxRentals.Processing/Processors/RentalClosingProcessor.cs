using System;
using System.Threading.Tasks;
using WaxRentals.Service.Shared.Connectors;
using WaxRentals.Service.Shared.Entities;
using WaxRentals.Waxp.Transact;

namespace WaxRentals.Processing.Processors
{
    internal class RentalClosingProcessor : Processor<Result<RentalInfo>>
    {

        private IRentalService Rentals { get; }
        private IWaxAccounts Wax { get; }

        public RentalClosingProcessor(ITrackService track, IRentalService rentals, IWaxAccounts wax)
            : base(track)
        {
            Rentals = rentals;
            Wax = wax;
        }

        protected override Func<Task<Result<RentalInfo>>> Get => Rentals.NextClosing;
        protected async override Task Process(Result<RentalInfo> result)
        {
            if (result.Success)
            {
                var rental = result.Value;
                var wax = Wax.GetAccount(rental.SourceAccount);
                var (success, hash) = await wax.Unstake(rental.WaxAccount, rental.Cpu, rental.Net);
                if (success)
                {
                    await Rentals.ProcessClosing(rental.Id, hash);
                }
            }
        }

    }
}
