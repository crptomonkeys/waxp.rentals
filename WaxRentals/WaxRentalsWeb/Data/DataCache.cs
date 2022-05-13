using System;
using Microsoft.Extensions.DependencyInjection;
using WaxRentals.Banano.Monitoring;
using WaxRentals.Monitoring.Prices;

namespace WaxRentalsWeb.Data
{
    internal class DataCache : IDataCache
    {

        public event EventHandler AppStateChanged;
        public AppState AppState { get; } = new();

        private readonly IPriceMonitor _prices;
        private readonly BalanceMonitor _banano;

        public DataCache(IPriceMonitor prices, BalanceMonitor banano)
        {
            _prices = prices;
            _banano = banano;
        }

        public void Initialize()
        {
            _prices.Updated += (_, _) =>
            {
                AppState.BananoPrice.Value = _prices.Banano;
                AppState.WaxPrice.Value = _prices.Wax;
                RaiseEvent();
            };

            _banano.Updated += (_, balance) =>
            {
                // If we just do (decimal)balance, we can only get whole numbers.
                AppState.BananoBalance.Value = decimal.Parse(balance.ToString());
                RaiseEvent();
            };
        }

        private void RaiseEvent()
        {
            AppStateChanged?.Invoke(this, EventArgs.Empty);
        }

    }

    public static class DataCacheExtensions
    {
        public static void UseDataCache(this IServiceProvider provider)
        {
            provider.GetRequiredService<DataCache>().Initialize();
        }
    }
}
