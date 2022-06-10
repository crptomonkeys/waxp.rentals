using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using WaxRentals.Data.Entities;
using WaxRentals.Data.Manager;
using WaxRentals.Monitoring.Extensions;

namespace WaxRentals.Monitoring.Recents
{
    internal class InisghtsMonitor : Monitor, IInsightsMonitor
    {

        private SafeEnumerable<Rental> _rentals = new();
        public IEnumerable<Rental> RecentRentals { get { return _rentals.SafeRead(); } }

        private SafeEnumerable<Purchase> _purchases = new();
        public IEnumerable<Purchase> RecentPurchases { get { return _purchases.SafeRead(); } }

        private SafeEnumerable<WelcomePackage> _packages = new();
        public IEnumerable<WelcomePackage> RecentWelcomePackages { get { return _packages.SafeRead(); } }

        private SafeEnumerable<MonthlyStats> _stats = new();
        public IEnumerable<MonthlyStats> MonthlyStats { get { return _stats.SafeRead(); } }

        public InisghtsMonitor(TimeSpan interval, IDataFactory factory)
            : base(interval, factory)
        {
            // Nothing additional.
        }

        protected override bool Tick()
        {
            var update = false;

            try
            {
                var rentals = Factory.Explore.GetRecentRentals();
                var purchases = Factory.Explore.GetRecentPurchases();
                var packages = Factory.Explore.GetRecentWelcomePackages();
                var stats = Factory.Explore.GetMonthlyStats();

                if (_rentals.UnsafeRead() == null || _purchases.UnsafeRead() == null || _packages.UnsafeRead() == null || _stats.UnsafeRead() == null)
                {
                    update = true;
                    _rentals.Write(rentals);
                    _purchases.Write(purchases);
                    _packages.Write(packages);
                    _stats.Write(stats);
                }
                else
                {
                    if (Differ(_rentals.UnsafeRead(), rentals, rental => rental.RentalId))
                    {
                        update = true;
                        _rentals.Write(rentals);
                    }

                    if (Differ(_purchases.UnsafeRead(), purchases, purchase => purchase.PurchaseId))
                    {
                        update = true;
                        _purchases.Write(purchases);
                    }

                    if (Differ(_packages.UnsafeRead(), packages))
                    {
                        update = true;
                        _packages.Write(packages);
                    }

                    if (Differ(_stats.UnsafeRead(), stats))
                    {
                        update = true;
                        _stats.Write(stats);
                    }
                }
            }
            catch (Exception ex)
            {
                Factory.Log.Error(ex);
            }

            return update;
        }

        #region " Differ "

        private bool Differ<T>(IEnumerable<T> left, IEnumerable<T> right, Func<T, int> get)
        {
            var leftIds = left.Select(get);
            var rightIds = right.Select(get);
            return leftIds.Except(rightIds).Any() || rightIds.Except(leftIds).Any();
        }

        private bool Differ(IEnumerable<WelcomePackage> left, IEnumerable<WelcomePackage> right)
        {
            var differ = Differ(left, right, package => package.PackageId);
            if (!differ)
            {
                differ = (from p1 in left
                          join p2 in right
                          on p1.PackageId equals p2.PackageId
                          where p1.NftTransaction != p2.NftTransaction || p1.RentalId != p2.RentalId
                          select 1).Any();
            }
            return differ;
        }

        private bool Differ(IEnumerable<MonthlyStats> left, IEnumerable<MonthlyStats> right)
        {
            var firstLeft = left.FirstOrDefault();
            var firstRight = right.FirstOrDefault();
            return firstLeft.Year != firstRight.Year ||
                   firstLeft.Month != firstRight.Month ||
                   firstLeft.WaxDaysRented != firstRight.WaxDaysRented ||
                   firstLeft.WaxDaysFree != firstRight.WaxDaysFree ||
                   firstLeft.WaxPurchasedForSite != firstRight.WaxPurchasedForSite ||
                   firstLeft.WelcomePackagesOpened != firstRight.WelcomePackagesOpened;
        }

        #endregion

        #region " SafeEnumerable "

        private class SafeEnumerable<T>
        {
            private IEnumerable<T> _value;
            private readonly ReaderWriterLockSlim _rwls = new();

            public void Write(IEnumerable<T> value) => _rwls.SafeWrite(() => _value = value);
            public IEnumerable<T> SafeRead() => _rwls.SafeRead(() => _value);
            public IEnumerable<T> UnsafeRead() => _value;
        }

        #endregion

    }
}
