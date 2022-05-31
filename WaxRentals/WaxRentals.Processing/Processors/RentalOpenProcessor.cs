using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WaxRentals.Banano.Transact;
using WaxRentals.Data.Entities;
using WaxRentals.Data.Manager;
using WaxRentals.Monitoring.Notifications;

namespace WaxRentals.Processing.Processors
{
    internal class RentalOpenProcessor : Processor<IEnumerable<Rental>>
    {

        protected override bool ProcessMultiplePerTick => false;

        private IBananoAccountFactory Banano { get; }
        private ITelegramNotifier Telegram { get; }

        public RentalOpenProcessor(IDataFactory factory, IBananoAccountFactory banano, ITelegramNotifier telegram)
            : base(factory)
        {
            Banano = banano;
            Telegram = telegram;
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
                    Telegram.Send($"Received rental payment for {rental.TargetWaxAccount}.");
                }
            }
            catch (Exception ex)
            {
                await Factory.Log.Error(ex, context: rental);
            }
        }

    }
}
