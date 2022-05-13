using System;

namespace WaxRentalsWeb.Data
{
    public interface IDataCache
    {

        event EventHandler AppStateChanged;
        AppState AppState { get; }

    }
}
