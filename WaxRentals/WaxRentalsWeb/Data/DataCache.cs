using System;
using System.Linq;
using Microsoft.Extensions.DependencyInjection;
using WaxRentals.Banano.Monitoring;
using WaxRentals.Monitoring;
using WaxRentals.Monitoring.Prices;
using WaxRentals.Monitoring.Recents;
using WaxRentals.Waxp.Monitoring;
using WaxRentalsWeb.Data.Models;
using WaxRentalsWeb.Files;

namespace WaxRentalsWeb.Data
{
    internal class DataCache : IDataCache
    {

        public event EventHandler AppStateChanged;
        public AppState AppState { get; } = new();

        private readonly IPriceMonitor _prices;
        private readonly BalanceMonitor _banano;
        private readonly BalancesMonitor _wax;
        private readonly NftsMonitor _nfts;
        private readonly FileMonitor _siteMessage;

        public event EventHandler RecentsChanged;
        public Recents Recents { get; } = new();

        private readonly IRecentMonitor _recent;

        public DataCache(
            IPriceMonitor prices,
            BalanceMonitor banano,
            BalancesMonitor wax,
            NftsMonitor nfts,
            SiteMessageMonitor siteMessage,
            IRecentMonitor recent)
        {
            _prices = prices;
            _banano = banano;
            _wax = wax;
            _nfts = nfts;
            _siteMessage = siteMessage;

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

            _nfts.Updated += (_, nfts) =>
            {
                var updated = AppState.WelcomePackageNftsAvailable != nfts.Any();
                if (updated)
                {
                    AppState.WelcomePackageNftsAvailable = nfts.Any();
                    RaiseAppStateEvent();
                }
            };

            _siteMessage.Updated += (_, message) =>
            {
                AppState.SiteMessage = message;
                RaiseAppStateEvent();
            };

            _prices.Initialize();
            _banano.Initialize();
            _wax.Initialize();
            _nfts.Initialize();
            _siteMessage.Initialize();

            _recent.Updated += (_, _) =>
            {
                Recents.Rentals = _recent.Rentals;
                Recents.Purchases = _recent.Purchases;
                Recents.WelcomePackages = _recent.WelcomePackages;
                RaiseRecentsEvent();
            };

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
