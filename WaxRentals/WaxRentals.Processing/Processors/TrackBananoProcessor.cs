using System;
using System.Threading.Tasks;
using WaxRentals.Service.Shared.Connectors;
using WaxRentals.Service.Shared.Entities;
using static WaxRentals.Service.Shared.Config.Constants;

namespace WaxRentals.Processing.Processors
{
    internal class TrackBananoProcessor : Processor<Result<decimal>>
    {

        private IBananoService Banano { get; }
        private IAppService App { get; }

        public TrackBananoProcessor(ITrackService track, IBananoService banano, IAppService app)
            : base(track)
        {
            Banano = banano;
            App = app;
        }

        protected override Func<Task<Result<decimal>>> Get => Banano.CompleteSweeps;
        protected async override Task<bool> Process(Result<decimal> result)
        {
            if (result.Success && result.Value > 0)
            {
                LogTransaction("Received BAN", result.Value, Coins.Banano, earned: decimal.Round(await ToUsd(result.Value), 2));
            }
            return false;
        }

        private async Task<decimal> ToUsd(decimal banano)
        {
            var result = await App.State();
            return banano * (result.Value?.BananoPrice ?? 0);
        }

    }
}
