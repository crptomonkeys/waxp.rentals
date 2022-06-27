using System;
using WaxRentalsWeb.Data.Models;

namespace WaxRentalsWeb.Data
{
    public interface IDataCache
    {

        event EventHandler InsightsChanged;
        Insights Insights { get; }

    }
}
