using System;
using WaxRentals.Service.Shared.Entities;

namespace WaxRentals.Monitoring.App
{
    public interface IAppInsightsMonitor
    {

        event EventHandler Updated;
        void Initialize();

        AppInsights Value { get; }

    }
}
