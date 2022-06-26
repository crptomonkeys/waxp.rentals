using System;
using System.Threading;
using WaxRentals.Data.Manager;
using WaxRentals.Monitoring.Extensions;
using WaxRentals.Service.Shared.Connectors;
using WaxRentals.Service.Shared.Entities;

namespace WaxRentals.Monitoring.Recents
{
    internal class AppStateMonitor : Monitor, IAppStateMonitor
    {

        private AppState _appState;
        private readonly ReaderWriterLockSlim _rwls = new();
        public AppState Value { get { return _rwls.SafeRead(() => _appState); } }

        public IAppService Service { get; }

        public AppStateMonitor(TimeSpan interval, IDataFactory factory, IAppService service)
            : base(interval, factory)
        {
            Service = service;
        }

        protected override bool Tick()
        {
            var update = false;

            try
            {
                var state = Service.State().GetAwaiter().GetResult().Value;
                if (_rwls.SafeRead(() => _appState) == null || Differ(_rwls.SafeRead(() => _appState), state))
                {
                    update = true;
                    _rwls.SafeWrite(() => _appState = state);
                }
            }
            catch (Exception ex)
            {
                Factory.Log.Error(ex);
            }

            return update;
        }

        #region " Differ "

        private bool Differ(AppState left, AppState right)
        {
            var comparison = StringComparison.OrdinalIgnoreCase;
            return !string.Equals(left.BananoAddress, right.BananoAddress, comparison)   ||
                   left.BananoBalance != right.BananoBalance                             ||
                   left.BananoPrice != right.BananoPrice                                 ||
                   !string.Equals(left.WaxAccount, right.WaxAccount, comparison)         ||
                   left.WaxBalanceAvailableToday != right.WaxBalanceAvailableToday       ||
                   left.WaxBalanceAvailableTomorrow != right.WaxBalanceAvailableTomorrow ||
                   left.WaxPrice != right.WaxPrice                                       ||
                   left.WaxStaked != right.WaxStaked                                     ||
                   !string.Equals(left.WaxWorkingAccount, right.WaxWorkingAccount, comparison);
        }

        #endregion

    }
}
