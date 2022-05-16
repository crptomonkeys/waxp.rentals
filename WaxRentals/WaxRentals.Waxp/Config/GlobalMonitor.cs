using System.Text.RegularExpressions;
using WaxRentals.Data.Entities;
using WaxRentals.Data.Manager;
using WaxRentals.Monitoring;
using WaxRentals.Monitoring.Logging;
using WaxRentals.Monitoring.Prices;
using WaxRentals.Waxp.Monitoring;
using static WaxRentals.Monitoring.Config.Constants;
using static WaxRentals.Waxp.Config.Constants;

namespace WaxRentals.Waxp.Config
{
    internal class GlobalMonitor : IGlobalMonitor
    {
        
        private static HistoryMonitor _monitor;
        private static readonly object _deadbolt = new();

        public GlobalMonitor(HistoryMonitor monitor, IInsert data, IPriceMonitor prices)
        {
            if (_monitor == null)
            {
                lock (_deadbolt)
                {
                    if (_monitor == null)
                    {
                        _monitor = monitor;
                        _monitor.Updated += async (sender, transfers) =>
                        {
                            foreach (var transfer in transfers)
                            {
                                var address = IsBananoAddress(transfer.Memo) ? transfer.Memo: null;
                                var skip = transfer.Amount >= Protocol.MinimumTransaction && address != null;
                                var banano = transfer.Amount * (prices.Wax / prices.Banano);
                                await data.OpenPurchase(transfer.Amount, transfer.Hash, address, banano, skip ? Status.Processed : Status.New);
                                Tracker.Track("Received WAX", transfer.Amount, Coins.Wax, earned: transfer.Amount * prices.Wax);
                            }
                        };
                        _monitor.Initialize();
                    }
                }
            }
        }

        private bool IsBananoAddress(string memo)
        {
            return Regex.IsMatch(memo ?? "", Protocol.BananoAddressRegex, RegexOptions.IgnoreCase);
        }

    }
}
