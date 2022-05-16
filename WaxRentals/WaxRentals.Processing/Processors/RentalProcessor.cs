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

        public RentalProcessor(IProcess data, ILog log, IWaxAccounts wax, IBananoAccountFactory banano)
            : base(data, log)
        {
            Wax = wax;
            Banano = banano;
        }

        protected override Func<Task<IEnumerable<Rental>>> Get => Data.PullNewRentals;
        protected override async Task Process(IEnumerable<Rental> rentals)
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
                    var wax = Wax.Today;
                    await Data.ProcessRentalPayment(rental.RentalId);
                    var (success, hash) = await wax.Stake(rental.TargetWaxAccount, rental.CPU, rental.NET);
                    if (success)
                    {
                        await Data.ProcessRentalStaking(rental.RentalId, wax.Account, hash);
                        await account.Receive(verifyOnly: false);
                    }
                }
            }
            catch (Exception ex)
            {
                await Log.Error(ex, context: rental);
            }
        }

    }
}
