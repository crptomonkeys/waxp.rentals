using System;
using System.Threading.Tasks;
using WaxRentals.Api.Entities.App;

namespace WaxRentalsWeb.Monitoring
{
    public interface IAppStateMonitor
    {

        event EventHandler Updated;
        Task Initialize();

        AppState Value { get; }

    }
}
