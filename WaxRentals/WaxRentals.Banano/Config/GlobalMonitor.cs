using System;
using WaxRentals.Banano.Monitoring;
using WaxRentals.Banano.Transact;
using WaxRentals.Monitoring;
using WaxRentals.Monitoring.Logging;
using WaxRentals.Monitoring.Prices;
using static WaxRentals.Monitoring.Config.Constants;

namespace WaxRentals.Banano.Config
{
    internal class GlobalMonitor : IGlobalMonitor
    {
        
        private static AccountMonitor _monitor;
        private static readonly object _deadbolt = new();

        public GlobalMonitor(StorageAccount storage, PriceMonitor prices)
        {
            if (_monitor == null)
            {
                lock (_deadbolt)
                {
                    if (_monitor == null)
                    {
                        _monitor = new AccountMonitor(TimeSpan.FromSeconds(10), storage);
                        _monitor.Updated += (sender, received) =>
                        {
                            // Add to queue to process (1. send to storage, 2. credit caller)
                            // except this isn't where it's needed -- that has to happen on the monitors for the other accounts
                            var converted = decimal.Parse(received.ToString()); // If we just do (decimal)received, we can only get whole numbers.
                            Tracker.Track("Received BAN", converted, Coins.Banano, earned: converted * prices.Banano);
                        };
                    }
                }
            }
        }

    }
}
