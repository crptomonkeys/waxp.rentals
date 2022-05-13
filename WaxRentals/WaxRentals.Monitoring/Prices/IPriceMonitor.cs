using System;

namespace WaxRentals.Monitoring.Prices
{
    public interface IPriceMonitor
    {

        event EventHandler Updated;

        decimal Banano { get; }
        decimal Wax { get; }

    }
}
