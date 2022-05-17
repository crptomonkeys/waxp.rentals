using System;
using System.IO;

namespace WaxRentals.Monitoring.Logging
{
    public static class Tracker
    {

        private static readonly object _deadbolt = new();

        public static void Track(string description, decimal quantity, string coin, decimal? earned = null, decimal? spent = null)
        {
            lock (_deadbolt)
            {
                File.AppendAllText(
                    "/run/output/tracking.csv",
                    $"{DateTime.Now:yyyy-MM-dd},{description},{quantity:0.0000} {coin},{earned:0.00},{spent:0.00}{Environment.NewLine}"
                );
            }
        }

    }
}
