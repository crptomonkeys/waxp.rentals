using System;
using System.Threading.Tasks;
using WaxRentals.Banano.Transact;
using WaxRentals.Data.Manager;
using WaxRentals.Monitoring.Prices;
using WaxRentals.Processing.Tracking;
using static WaxRentals.Monitoring.Config.Constants;

namespace WaxRentals.Processing.Processors
{
    internal class TrackBananoProcessor : Processor<decimal>
    {

        protected override bool ProcessMultiplePerTick => false;

        private IBananoAccount Banano { get; }
        private IPriceMonitor Prices { get; }
        private ITracker Tracker { get; }

        public TrackBananoProcessor(IDataFactory factory, IBananoAccount banano, IPriceMonitor prices, ITracker tracker)
            : base(factory)
        {
            Banano = banano;
            Prices = prices;
            Tracker = tracker;
        }

        protected override Func<Task<decimal>> Get => () => Banano.Receive();
        protected override Task Process(decimal received)
        {
            if (received > 0)
            {
                var converted = decimal.Parse(received.ToString()); // If we just do (decimal)received, we can only get whole numbers.
                Tracker.Track("Received BAN", converted, Coins.Banano, earned: decimal.Round(converted * Prices.Banano, 2));
            }
            return Task.CompletedTask;
        }

    }
}
