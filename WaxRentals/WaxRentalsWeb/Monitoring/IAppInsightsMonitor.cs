using System;
using System.Threading.Tasks;
using WaxRentals.Api.Entities.App;

namespace WaxRentalsWeb.Monitoring
{
    public interface IAppInsightsMonitor
    {

        event EventHandler Updated;
        Task Initialize();

        AppInsights Value { get; }

    }
}
