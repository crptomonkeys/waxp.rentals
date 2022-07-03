using System;
using System.Threading.Tasks;
using WaxRentals.Banano.Transact;
using WaxRentals.Monitoring.Prices;
using WaxRentals.Service.Shared.Connectors;
using static WaxRentals.Service.Shared.Config.Constants;

namespace WaxRentals.Processing.Processors
{
    internal class TrackBananoProcessor : Processor<decimal>
    {

        protected override bool ProcessMultiplePerTick => false;

        private IBananoAccount Banano { get; }
        private IPriceMonitor Prices { get; }

        public TrackBananoProcessor(ITrackService track, IBananoAccount banano, IPriceMonitor prices)
            : base(track)
        {
            Banano = banano;
            Prices = prices;
        }

        protected override Func<Task<decimal>> Get => () => Banano.Receive();
        protected override Task Process(decimal received)
        {
            if (received > 0)
            {
                var converted = decimal.Parse(received.ToString()); // If we just do (decimal)received, we can only get whole numbers.
                LogTransaction("Received BAN", converted, Coins.Banano, earned: decimal.Round(converted * Prices.Banano, 2));
            }
            return Task.CompletedTask;
        }

    }
}
