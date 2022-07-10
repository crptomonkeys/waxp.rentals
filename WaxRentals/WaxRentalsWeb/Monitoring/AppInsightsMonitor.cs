using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using WaxRentals.Api.Entities.App;
using WaxRentals.Api.Entities.WelcomePackages;
using WaxRentals.Service.Shared.Connectors;
using WaxRentalsWeb.Extensions;
using WaxRentalsWeb.Net;

namespace WaxRentalsWeb.Monitoring
{
    internal class AppInsightsMonitor : Monitor, IAppInsightsMonitor
    {

        private AppInsights _insights;
        private readonly ReaderWriterLockSlim _rwls = new();
        public AppInsights Value { get { return _rwls.SafeRead(() => _insights); } }

        public ApiProxy Proxy { get; }

        public AppInsightsMonitor(TimeSpan interval, ITrackService log, ApiProxy proxy)
            : base(interval, log)
        {
            Proxy = proxy;
        }

        protected async override Task<bool> Tick()
        {
            var update = false;

            try
            {
                var result = await Proxy.Get<AppInsights>(Proxy.Endpoints.AppInsights);
                if (result.Success)
                {
                    var insights = result.Value;
                    if (_rwls.SafeRead(() => _insights) == null || Differ(_rwls.SafeRead(() => _insights), insights))
                    {
                        update = true;
                        _rwls.SafeWrite(() => _insights = insights);
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

        private bool Differ(AppInsights left, AppInsights right)
        {
            return Differ(left.MonthlyStats         , right.MonthlyStats                                                 ) ||
                   Differ(left.LatestRentals        , right.LatestRentals        , rental => rental.Payment.Address      ) ||
                   Differ(left.LatestPurchases      , right.LatestPurchases      , purchase => purchase.BananoTransaction) ||
                   Differ(left.LatestWelcomePackages, right.LatestWelcomePackages                                        );
        }

        private bool Differ<T>(IEnumerable<T> left, IEnumerable<T> right, Func<T, string> get)
        {
            if (left == null || right == null)
            {
                return true;
            }

            var leftIds = left.Select(get);
            var rightIds = right.Select(get);
            return !Enumerable.SequenceEqual(leftIds, rightIds, StringComparer.OrdinalIgnoreCase);
        }

        private bool Differ(IEnumerable<WelcomePackageInfo> left, IEnumerable<WelcomePackageInfo> right)
        {
            var differ = Differ(left, right, package => package.Payment.Address);
            if (!differ)
            {
                differ = (from p1 in left
                          join p2 in right
                          on p1.Payment.Address equals p2.Payment.Address
                          where p1.Transactions.NftTransfer != p2.Transactions.NftTransfer || p1.Transactions.RentalStake != p2.Transactions.RentalStake
                          select 1).Any();
            }
            return differ;
        }

        private bool Differ(IEnumerable<MonthlyStats> left, IEnumerable<MonthlyStats> right)
        {
            var firstLeft = left?.FirstOrDefault();
            var firstRight = right?.FirstOrDefault();
            return firstLeft?.Year != firstRight?.Year ||
                   firstLeft?.Month != firstRight?.Month ||
                   firstLeft?.WaxDaysRented != firstRight?.WaxDaysRented ||
                   firstLeft?.WaxDaysFree != firstRight?.WaxDaysFree ||
                   firstLeft?.WaxPurchasedForSite != firstRight?.WaxPurchasedForSite ||
                   firstLeft?.WelcomePackagesOpened != firstRight?.WelcomePackagesOpened;
        }

        #endregion

    }
}
