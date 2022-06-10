using System;
using WaxRentalsWeb.Data.Models;

namespace WaxRentalsWeb.Data
{
    public interface IDataCache
    {

        event EventHandler AppStateChanged;
        AppState AppState { get; }

        event EventHandler InsightsChanged;
        Insights Insights { get; }

    }
}
