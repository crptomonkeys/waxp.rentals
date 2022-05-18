using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WaxRentals.Banano.Transact;
using WaxRentals.Data.Entities;
using WaxRentals.Data.Manager;
using WaxRentals.Waxp.Transact;
using static WaxRentals.Banano.Config.Constants;

namespace WaxRentals.Processing.Processors
{
    internal class RentalProcessor : Processor<IEnumerable<Rental>>
    {

        private IWaxAccounts Wax { get; }
        private IBananoAccountFactory Banano { get; }

        public RentalProcessor(IDataFactory factory, IWaxAccounts wax, IBananoAccountFactory banano)
            : base(factory)
        {
            Wax = wax;
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
                var pending = await account.Receive(verifyOnly: true);
                pending /= Math.Pow(10, Protocol.Decimals);
                if (rental.Banano >= pending)
                {
                    await Factory.Process.ProcessRentalPayment(rental.RentalId);

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
                        await account.Receive(verifyOnly: false);
                    }
                }
            }
            catch (Exception ex)
            {
                await Factory.Log.Error(ex, context: rental);
            }
        }

    }
}
