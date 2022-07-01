using System;
using System.Threading;
using WaxRentals.Data.Manager;
using WaxRentals.Service.Shared.Connectors;
using WaxRentals.Service.Shared.Entities;
using WaxRentalsWeb.Extensions;

namespace WaxRentalsWeb.Monitoring
{
    internal class AppStateMonitor : Monitor, IAppStateMonitor
    {

        private AppState _state;
        private readonly ReaderWriterLockSlim _rwls = new();
        public AppState Value { get { return _rwls.SafeRead(() => _state); } }

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
                if (_rwls.SafeRead(() => _state) == null || Differ(_rwls.SafeRead(() => _state), state))
                {
                    update = true;
                    _rwls.SafeWrite(() => _state = state);
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
            return !string.Equals(left.BananoAddress, right.BananoAddress, comparison)         ||
                   left.BananoBalance != right.BananoBalance                                   ||
                   left.BananoPrice != right.BananoPrice                                       ||
                                                                                               
                   !string.Equals(left.WaxAccount, right.WaxAccount, comparison)               ||
                   left.WaxBalanceAvailableToday != right.WaxBalanceAvailableToday             ||
                   left.WaxBalanceAvailableTomorrow != right.WaxBalanceAvailableTomorrow       ||
                   left.WaxPrice != right.WaxPrice                                             ||
                   left.WaxStaked != right.WaxStaked                                           ||
                   !string.Equals(left.WaxWorkingAccount, right.WaxWorkingAccount, comparison) || 
                   
                   left.WaxRentPriceInBanano != right.WaxRentPriceInBanano                     ||
                   left.WaxBuyPriceInBanano != right.WaxBuyPriceInBanano                       ||
                   left.BananoWelcomePackagePrice != right.BananoWelcomePackagePrice           ||
                   
                   left.BananoMinimumCredit != right.BananoMinimumCredit                       ||
                   left.WaxMinimumRent != right.WaxMinimumRent                                 ||
                   left.WaxMaximumRent != right.WaxMaximumRent                                 ||
                   left.WaxMinimumBuy != right.WaxMinimumBuy                                   ||
                   left.WaxMaximumBuy != right.WaxMaximumBuy                                   ||
                   
                   left.WelcomePackagesAvailable != right.WelcomePackagesAvailable             ||
                   left.WelcomePackageRentalsAvailable != right.WelcomePackageRentalsAvailable ||
                   left.WelcomePackageNftsAvailable != right.WelcomePackageNftsAvailable;
        }

        #endregion

    }
}
