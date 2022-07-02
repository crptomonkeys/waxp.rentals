using System;
using System.Threading.Tasks;
using WaxRentals.Service.Shared.Entities;

namespace WaxRentalsWeb.Monitoring
{
    public interface IAppInsightsMonitor
    {

        event EventHandler Updated;
        Task Initialize();

        AppInsights Value { get; }

    }
}
