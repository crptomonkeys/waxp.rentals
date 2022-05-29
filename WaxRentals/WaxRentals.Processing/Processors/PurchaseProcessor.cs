using System;
using System.Threading.Tasks;
using WaxRentals.Data.Entities;
using WaxRentals.Data.Manager;
using WaxRentals.Monitoring.Prices;
using WaxRentals.Processing.Tracking;
using WaxRentals.Waxp.Transact;
using static WaxRentals.Monitoring.Config.Constants;
using BananoAccount = WaxRentals.Banano.Transact.IBananoAccount;

namespace WaxRentals.Processing.Processors
{
    internal class PurchaseProcessor : Processor<Purchase>
    {

        private BananoAccount Banano { get; }
        private IWaxAccounts Wax { get; }
        private IPriceMonitor Prices { get; }
        private ITracker Tracker { get; }

        public PurchaseProcessor(IDataFactory factory, BananoAccount banano, IWaxAccounts wax, IPriceMonitor prices, ITracker tracker)
            : base(factory)
        {
            Banano = banano;
            Wax = wax;
            Prices = prices;
            Tracker = tracker;
        }

        protected override Func<Task<Purchase>> Get => Factory.Process.PullNextPurchase;
        protected async override Task Process(Purchase purchase)
        {
            var hash = await Banano.Send(purchase.PaymentBananoAddress, purchase.Banano);
            var dataTask = Factory.Process.ProcessPurchase(purchase.PurchaseId, hash);
            var waxTask = Wax.Primary.Send(Wax.Today.Account, purchase.Wax);
            Tracker.Track("Sent BAN", purchase.Banano, Coins.Banano, spent: purchase.Banano * Prices.Banano);
            await Task.WhenAll(dataTask, waxTask);
        }

    }
}
