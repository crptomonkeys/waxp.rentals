using WaxRentals.Monitoring;
using WaxRentals.Waxp.Monitoring;

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

                        };
                        _monitor.Initialize();
                    }
                }
            }
        }

    }
}
