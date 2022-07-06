using WaxRentals.Data.Manager;
using File = DockerHax.IO.File;

namespace WaxRentals.Service.Tracking
{
    internal class Tracker : ITracker
    {

        private ILog Log { get; }

        public Tracker(ILog log)
        {
            Log = log;
        }

        private static readonly object _deadbolt = new();
        public void Track(string description, decimal quantity, string coin, decimal? earned = null, decimal? spent = null)
        {
            lock (_deadbolt)
            {
                try
                {
                    File.AppendAllText(
                        "/run/output/tracking.csv",
                        $"{DateTime.Now:yyyy-MM-dd},{description},{quantity:0.0000} {coin},{earned:0.00},{spent:0.00}{Environment.NewLine}"
                    );
                }
                catch (Exception ex)
                {
                    Log.Error(ex);
                }
            }
        }

    }
}
