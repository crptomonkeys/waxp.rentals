using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using WaxRentals.Data.Manager;
using WaxRentals.Monitoring.Extensions;
using WaxRentals.Service.Shared.Connectors;
using WaxRentals.Service.Shared.Entities;

namespace WaxRentals.Monitoring.App
{
    internal class AppInsightsMonitor : Monitor, IAppInsightsMonitor
    {

        private AppInsights _insights;
        private readonly ReaderWriterLockSlim _rwls = new();
        public AppInsights Value { get { return _rwls.SafeRead(() => _insights); } }

        public IAppService Service { get; }

        public AppInsightsMonitor(TimeSpan interval, IDataFactory factory, IAppService service)
            : base(interval, factory)
        {
            Service = service;
        }

        protected override bool Tick()
        {
            var update = false;

            try
            {
                var insights = Service.Insights().GetAwaiter().GetResult().Value;
                if (_rwls.SafeRead(() => _insights) == null || Differ(_rwls.SafeRead(() => _insights), insights))
                {
                    update = true;
                    _rwls.SafeWrite(() => _insights = insights);
                }
            }
            catch (Exception ex)
            {
                Factory.Log.Error(ex);
            }

            return update;
        }

        #region " Differ "

        private bool Differ(AppInsights left, AppInsights right)
        {
            return Differ(left.MonthlyStats         , right.MonthlyStats                                                 ) ||
                   Differ(left.LatestRentals        , right.LatestRentals        , rental => rental.BananoAddress        ) ||
                   Differ(left.LatestPurchases      , right.LatestPurchases      , purchase => purchase.BananoTransaction) ||
                   Differ(left.LatestWelcomePackages, right.LatestWelcomePackages                                        );
        }

        private bool Differ<T>(IEnumerable<T> left, IEnumerable<T> right, Func<T, string> get)
        {
            var leftIds = left.Select(get);
            var rightIds = right.Select(get);
            return Enumerable.SequenceEqual(leftIds, rightIds, StringComparer.OrdinalIgnoreCase);
        }

        private bool Differ(IEnumerable<WelcomePackageInfo> left, IEnumerable<WelcomePackageInfo> right)
        {
            var differ = Differ(left, right, package => package.BananoAddress);
            if (!differ)
            {
                differ = (from p1 in left
                          join p2 in right
                          on p1.BananoAddress equals p2.BananoAddress
                          where p1.NftTransaction != p2.NftTransaction || p1.StakeTransaction != p2.StakeTransaction
                          select 1).Any();
            }
            return differ;
        }

        private bool Differ(IEnumerable<MonthlyStats> left, IEnumerable<MonthlyStats> right)
        {
            var firstLeft = left.FirstOrDefault();
            var firstRight = right.FirstOrDefault();
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
