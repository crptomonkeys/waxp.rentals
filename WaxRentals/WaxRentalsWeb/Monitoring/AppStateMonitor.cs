using System;
using System.Threading;
using System.Threading.Tasks;
using WaxRentals.Api.Entities.App;
using WaxRentals.Service.Shared.Connectors;
using WaxRentalsWeb.Extensions;
using WaxRentalsWeb.Net;

namespace WaxRentalsWeb.Monitoring
{
    internal class AppStateMonitor : Monitor, IAppStateMonitor
    {

        private AppState _state;
        private readonly ReaderWriterLockSlim _rwls = new();
        public AppState Value { get { return _rwls.SafeRead(() => _state); } }

        public ApiProxy Proxy { get; }

        public AppStateMonitor(TimeSpan interval, ITrackService log, ApiProxy proxy)
            : base(interval, log)
        {
            Proxy = proxy;
        }

        protected async override Task<bool> Tick()
        {
            var update = false;

            try
            {
                var result = await Proxy.Get<AppState>(Proxy.Endpoints.AppState);
                if (result.Success)
                {
                    var state = result.Value;
                    if (_rwls.SafeRead(() => _state) == null || Differ(_rwls.SafeRead(() => _state), state))
                    {
                        update = true;
                        _rwls.SafeWrite(() => _state = state);
                    }
                }
            }
            catch (Exception ex)
            {
                await Log.Error(ex);
            }

            return update;
        }

        #region " Differ "

        private static bool Differ(AppState left, AppState right)
        {
            var comparison = StringComparison.OrdinalIgnoreCase;
            return left.Banano.Balance                      != right.Banano.Balance                      ||
                   left.Banano.Price                        != right.Banano.Price                        ||
                                                                                               
                   left.Wax.AvailableToday                  != right.Wax.AvailableToday                  ||
                   left.Wax.AdditionalAvailableTomorrow     != right.Wax.AdditionalAvailableTomorrow     ||
                   left.Wax.Price                           != right.Wax.Price                           ||
                   left.Wax.Staked                          != right.Wax.Staked                          ||
    !string.Equals(left.Wax.WorkingAccount,                    right.Wax.WorkingAccount    , comparison) || 
                   
                   left.Costs.WaxRentPriceInBanano          != right.Costs.WaxRentPriceInBanano          ||
                   left.Costs.WaxBuyPriceInBanano           != right.Costs.WaxBuyPriceInBanano           ||
                   left.Costs.WelcomePackagePriceInBanano   != right.Costs.WelcomePackagePriceInBanano   ||
                   
                   left.Limits.BananoMinimumCredit          != right.Limits.BananoMinimumCredit          ||
                   left.Limits.WaxMinimumRent               != right.Limits.WaxMinimumRent               ||
                   left.Limits.WaxMaximumRent               != right.Limits.WaxMaximumRent               ||
                   left.Limits.WaxMinimumBuy                != right.Limits.WaxMinimumBuy                ||
                   left.Limits.WaxMaximumBuy                != right.Limits.WaxMaximumBuy                ||
                   
                   left.WelcomePackages.WaxAvailable        != right.WelcomePackages.WaxAvailable        ||
                   left.WelcomePackages.FreeRentalAvailable != right.WelcomePackages.FreeRentalAvailable ||
                   left.WelcomePackages.FreeNftAvailable    != right.WelcomePackages.FreeNftAvailable;
        }

        #endregion

    }
}
