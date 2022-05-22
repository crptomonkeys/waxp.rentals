using System;
using System.Threading.Tasks;
using Nano.Net.Numbers;
using WaxRentals.Banano.Transact;
using WaxRentals.Data.Manager;
using WaxRentals.Monitoring.Logging;
using WaxRentals.Monitoring.Prices;
using static WaxRentals.Banano.Config.Constants;
using static WaxRentals.Monitoring.Config.Constants;

namespace WaxRentals.Processing.Processors
{
    internal class TrackBananoProcessor : Processor<BigDecimal>
    {

        protected override bool ProcessMultiplePerTick => false;

        private IBananoAccount Banano { get; }
        private IPriceMonitor Prices { get; }

        public TrackBananoProcessor(IDataFactory factory, IBananoAccount banano, IPriceMonitor prices)
            : base(factory)
        {
            Banano = banano;
            Prices = prices;
        }

        protected override Func<Task<BigDecimal>> Get => () => Banano.Receive(verifyOnly: false);
        protected override Task Process(BigDecimal received)
        {
            received *= (1 / Math.Pow(10, Protocol.Decimals));
            if (received > 0)
            {
                var converted = decimal.Parse(received.ToString()); // If we just do (decimal)received, we can only get whole numbers.
                Tracker.Track("Received BAN", converted, Coins.Banano, earned: decimal.Round(converted * Prices.Banano, 2));
            }
            return Task.CompletedTask;
        }

    }
}
