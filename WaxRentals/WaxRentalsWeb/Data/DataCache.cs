using System;
using Microsoft.Extensions.DependencyInjection;
using WaxRentals.Monitoring.Recents;
using WaxRentalsWeb.Data.Models;

namespace WaxRentalsWeb.Data
{
    internal class DataCache : IDataCache
    {

        public event EventHandler InsightsChanged;
        public Insights Insights { get; } = new();

        private readonly IInsightsMonitor _insights;

        public DataCache(
            IInsightsMonitor insights)
        {
            _insights = insights;
        }

        public void Initialize()
        {
            _insights.Updated += (_, _) =>
            {
                Insights.RecentRentals         = _insights.RecentRentals;
                Insights.RecentPurchases       = _insights.RecentPurchases;
                Insights.RecentWelcomePackages = _insights.RecentWelcomePackages;
                Insights.MonthlyStats          = _insights.MonthlyStats;
                RaiseRecentsEvent();
            };

            _insights.Initialize();
        }

        private void RaiseRecentsEvent()
        {
            InsightsChanged?.Invoke(this, EventArgs.Empty);
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
