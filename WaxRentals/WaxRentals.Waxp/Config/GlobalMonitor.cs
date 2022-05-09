using System.Text.RegularExpressions;
using WaxRentals.Monitoring;
using WaxRentals.Waxp.Monitoring;
using static WaxRentals.Waxp.Config.Constants;

namespace WaxRentals.Waxp.Config
{
    internal class GlobalMonitor : IGlobalMonitor
    {
        
        private static AccountMonitor _monitor;
        private static readonly object _deadbolt = new();

        public GlobalMonitor(AccountMonitor monitor)
        {
            if (_monitor == null)
            {
                lock (_deadbolt)
                {
                    if (_monitor == null)
                    {
                        _monitor = monitor;
                        _monitor.Updated += (sender, transactions) =>
                        {
                            foreach (var (amount, memo) in transactions)
                            {
                                if (amount >= Protocol.MinimumTransaction &&
                                    Regex.IsMatch(memo ?? "", Protocol.BananoAddressRegex, RegexOptions.IgnoreCase))
                                {
                                    // Add to queue to process.
                                    //Tracker.Track
                                }
                            }
                        };
                        _monitor.Initialize();
                    }
                }
            }
        }

    }
}
