using System;
using Microsoft.Extensions.DependencyInjection;
using WaxRentals.Banano.Monitoring;
using WaxRentals.Monitoring.Prices;
using WaxRentals.Monitoring.Recents;
using WaxRentals.Waxp.Monitoring;
using WaxRentalsWeb.Data.Models;
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

        public event EventHandler RecentsChanged;
        public Recents Recents { get; } = new();

        private readonly IRecentMonitor _recent;

        public DataCache(IPriceMonitor prices, BalanceMonitor banano, BalancesMonitor wax, IRecentMonitor recent)
        {
            _prices = prices;
            _banano = banano;
            _wax = wax;
            _recent = recent;
        }

        public void Initialize()
        {
            _prices.Updated += (_, _) =>
            {
                AppState.BananoPrice.Value = Math.Round(_prices.Banano, 6);
                AppState.WaxPrice.Value = Math.Round(_prices.Wax, 6);
                RaiseAppStateEvent();
            };

            _banano.Updated += (_, balance) =>
            {
                AppState.BananoBalance.Value = Math.Round(balance, 4);
                RaiseAppStateEvent();
            };

            _wax.Updated += (_, balances) =>
            {
                AppState.WaxBalanceAvailable.Value = Math.Round(balances.Available, 4);
                AppState.WaxBalanceStaked.Value = Math.Round(balances.Staked, 4);
                AppState.WaxBalanceUnstaking.Value = Math.Round(balances.Unstaking, 4);
                RaiseAppStateEvent();
            };

            _recent.Updated += (_, _) =>
            {
                Recents.Rentals = _recent.Rentals;
                Recents.Purchases = _recent.Purchases;
                RaiseRecentsEvent();
            };

            _prices.Initialize();
            _banano.Initialize();
            _wax.Initialize();
            _recent.Initialize();
        }

        private void RaiseAppStateEvent()
        {
            AppStateChanged?.Invoke(this, EventArgs.Empty);
        }

        private void RaiseRecentsEvent()
        {
            RecentsChanged?.Invoke(this, EventArgs.Empty);
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
