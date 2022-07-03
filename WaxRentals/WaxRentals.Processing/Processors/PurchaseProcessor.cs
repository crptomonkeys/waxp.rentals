using System;
using System.Threading.Tasks;
using WaxRentals.Service.Shared.Connectors;
using WaxRentals.Service.Shared.Entities;
using static WaxRentals.Service.Shared.Config.Constants;

namespace WaxRentals.Processing.Processors
{
    internal class PurchaseProcessor : Processor<Result<PurchaseInfo>>
    {

        private IPurchaseService Purchases { get; }
        private IBananoService Banano { get; }
        private IAppService App { get; }

        public PurchaseProcessor(ITrackService track, IPurchaseService purchases, IBananoService banano, IAppService app)
            : base(track)
        {
            Purchases = purchases;
            Banano = banano;
            App = app;
        }

        protected override Func<Task<Result<PurchaseInfo>>> Get => Purchases.Next;

        protected async override Task Process(Result<PurchaseInfo> result)
        {
            if (result.Success)
            {
                await Process(result.Value);
            }
        }

        private async Task Process(PurchaseInfo purchase)
        {
            var result = await Banano.Send(purchase.BananoAddress, purchase.Banano);
            if (result.Success)
            {
                var dataTask = Purchases.Process(purchase.Id, result.Value);
                LogTransaction("Sent BAN", purchase.Banano, Coins.Banano, spent: await ToUsd(purchase.Banano));
                await dataTask;
            }
        }

        private async Task<decimal> ToUsd(decimal banano)
        {
            var result = await App.State();
            return banano * (result.Value?.BananoPrice ?? 0);
        }

    }
}
