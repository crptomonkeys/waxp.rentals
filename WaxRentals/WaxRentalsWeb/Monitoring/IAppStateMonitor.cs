using System;
using System.Threading.Tasks;
using WaxRentals.Service.Shared.Entities;

namespace WaxRentalsWeb.Monitoring
{
    public interface IAppStateMonitor
    {

        event EventHandler Updated;
        Task Initialize();

        AppState Value { get; }

    }
}
