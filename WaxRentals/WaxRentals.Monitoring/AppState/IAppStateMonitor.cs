using System;
using WaxRentals.Service.Shared.Entities;

namespace WaxRentals.Monitoring.Recents
{
    public interface IAppStateMonitor
    {

        event EventHandler Updated;
        void Initialize();

        AppState Value { get; }

    }
}
