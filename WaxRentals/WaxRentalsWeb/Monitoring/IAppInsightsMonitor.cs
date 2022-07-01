using System;
using WaxRentals.Service.Shared.Entities;

namespace WaxRentalsWeb.Monitoring
{
    public interface IAppInsightsMonitor
    {

        event EventHandler Updated;
        void Initialize();

        AppInsights Value { get; }

    }
}
