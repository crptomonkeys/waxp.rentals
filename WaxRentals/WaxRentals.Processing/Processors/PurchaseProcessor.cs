using System;
using System.Threading.Tasks;
using WaxRentals.Data.Entities;
using WaxRentals.Data.Manager;
using WaxRentals.Waxp.Transact;
using BananoAccount = WaxRentals.Banano.Transact.IBananoAccount;

namespace WaxRentals.Processing.Processors
{
    internal class PurchaseProcessor : Processor<Purchase>
    {

        private BananoAccount Banano { get; }
        private IWaxAccounts Wax { get; }

        public PurchaseProcessor(IProcess data, ILog log, BananoAccount banano, IWaxAccounts wax)
            : base(data, log)
        {
            Banano = banano;
            Wax = wax;
        }

        protected override Func<Task<Purchase>> Get => Data.PullNextPurchase;
        protected override async Task Process(Purchase purchase)
        {
            var hash = await Banano.Send(purchase.PaymentBananoAddress, purchase.Banano);
            await Data.ProcessPurchase(purchase.PurchaseId, hash);
            await Wax.Primary.Send(Wax.Today.Account, purchase.Wax);
        }

    }
}
