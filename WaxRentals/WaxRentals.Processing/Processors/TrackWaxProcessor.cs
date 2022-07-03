using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WaxRentals.Service.Shared.Connectors;
using WaxRentals.Service.Shared.Entities;
using Constants = WaxRentals.Service.Shared.Config.Constants;

namespace WaxRentals.Processing.Processors
{
    internal class TrackWaxProcessor : Processor<(IEnumerable<WaxTransferInfo>, AppState)>
    {

        protected override bool ProcessMultiplePerTick => false;

        private IWaxService Wax { get; }
        private IPurchaseService Purchases { get; }
        private IAppService App { get; }

        public TrackWaxProcessor(ITrackService track, IWaxService wax, IPurchaseService purchases, IAppService app)
            : base(track)
        {
            Wax = wax;
            Purchases = purchases;
            App = app;
        }

        protected override Func<Task<(IEnumerable<WaxTransferInfo>, AppState)>> Get => PullLatestHistory;

        internal async Task<(IEnumerable<WaxTransferInfo>, AppState)> PullLatestHistory()
        {
            // Only bother if we have a pay rate.  Otherwise, wait until we have one.
            var state = await App.State();
            if (state.Success && state.Value.WaxBuyPriceInBanano > 0)
            {
                var result = await Wax.LatestTransfers();
                if (result.Success)
                {
                    return (result.Value, state.Value);
                }
            }
            return (Enumerable.Empty<WaxTransferInfo>(), state.Value);
        }

        protected async override Task Process((IEnumerable<WaxTransferInfo>, AppState) info)
        {
            var (transfers, state) = info;

            var sweep = false;
            foreach (var transfer in transfers)
            {
                var banano = transfer.Amount * state.WaxBuyPriceInBanano;
                var result = await Purchases.Create(
                    transfer.Amount,
                    transfer.Transaction,
                    transfer.BananoPaymentAddress,
                    banano,
                    transfer.SkipPayment ? Status.Processed : Status.New);
                if (result.Success)
                {
                    LogTransaction("Received WAX", transfer.Amount, Constants.Coins.Wax, earned: transfer.Amount * state.WaxPrice);
                    Notify(transfer.SkipPayment
                        ? $"Received {transfer.Amount} {Constants.Coins.Wax} from {transfer.Source}."
                        : $"Bought {transfer.Amount} {Constants.Coins.Wax} off {transfer.Source}.");
                    sweep = true;
                }
            }

            if (sweep)
            {
                await Wax.Sweep();
            }
        }

    }
}
