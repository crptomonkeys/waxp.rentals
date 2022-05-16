using System;
using Microsoft.Extensions.DependencyInjection;
using WaxRentals.Banano.Monitoring;
using WaxRentals.Monitoring.Prices;
using WaxRentals.Waxp.Monitoring;
using static WaxRentals.Banano.Config.Constants;

namespace WaxRentalsWeb.Data
{
    internal class DataCache : IDataCache
    {

        public event EventHandler AppStateChanged;
        public AppState AppState { get; } = new();

        private readonly IPriceMonitor _prices;
        private readonly BalanceMonitor _banano;
        private readonly BalancesMonitor _wax;

        public DataCache(IPriceMonitor prices, BalanceMonitor banano, BalancesMonitor wax)
        {
            _prices = prices;
            _banano = banano;
            _wax = wax;
        }

        public void Initialize()
        {
            _prices.Updated += (_, _) =>
            {
                AppState.BananoPrice.Value = Math.Round(_prices.Banano, 6);
                AppState.WaxPrice.Value = Math.Round(_prices.Wax, 6);
                RaiseEvent();
            };

            _banano.Updated += (_, balance) =>
            {
                AppState.BananoBalance.Value = Math.Round((decimal)(balance / Math.Pow(10, Protocol.Decimals)), 4);
                RaiseEvent();
            };

            _wax.Updated += (_, balances) =>
            {
                AppState.WaxBalanceAvailable.Value = Math.Round(balances.Available, 4);
                AppState.WaxBalanceStaked.Value = Math.Round(balances.Staked, 4);
                AppState.WaxBalanceUnstaking.Value = Math.Round(balances.Unstaking, 4);
                RaiseEvent();
            };

            _prices.Initialize();
            _banano.Initialize();
            _wax.Initialize();
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
