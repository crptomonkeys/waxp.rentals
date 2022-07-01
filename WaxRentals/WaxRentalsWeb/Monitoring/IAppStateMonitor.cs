using System;
using WaxRentals.Service.Shared.Entities;

namespace WaxRentalsWeb.Monitoring
{
    public interface IAppStateMonitor
    {

        event EventHandler Updated;
        void Initialize();

        AppState Value { get; }

    }
}
