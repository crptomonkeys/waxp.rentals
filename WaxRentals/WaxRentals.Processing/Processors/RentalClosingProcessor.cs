using System;
using System.Threading.Tasks;
using WaxRentals.Banano.Transact;
using WaxRentals.Data.Entities;
using WaxRentals.Data.Manager;
using WaxRentals.Waxp.Transact;

namespace WaxRentals.Processing.Processors
{
    internal class RentalClosingProcessor : Processor<Rental>
    {

        private IWaxAccounts Wax { get; }
        private IBananoAccountFactory Banano { get; set; }

        public RentalClosingProcessor(IDataFactory factory, IWaxAccounts wax, IBananoAccountFactory banano)
            : base(factory)
        {
            Wax = wax;
            Banano = banano;
        }

        protected override Func<Task<Rental>> Get => Factory.Process.PullNextClosingRental;
        protected async override Task Process(Rental rental)
        {
            var wax = Wax.GetAccount(rental.SourceWaxAccount);
            var (success, hash) = await wax.Unstake(rental.TargetWaxAccount, rental.CPU, rental.NET);
            if (success)
            {
                await Factory.Process.ProcessRentalClosing(rental.RentalId, hash);
            }
        }

    }
}
