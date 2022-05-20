using System;
using System.Collections.Generic;
using WaxRentals.Data.Entities;

namespace WaxRentals.Monitoring.Recents
{
    public interface IRecentMonitor
    {

        event EventHandler Updated;
        void Initialize();

        IEnumerable<Rental> Rentals { get; }
        IEnumerable<Purchase> Purchases { get; }

    }
}
