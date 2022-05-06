using System;
using System.Timers;

namespace WaxRentals.Monitoring
{
    public abstract class Monitor : Updatable, IDisposable
    {

        private readonly Timer _timer;

        protected Monitor(TimeSpan interval)
        {
            _timer = new Timer(interval.TotalMilliseconds);
            _timer.Elapsed += (_, _) => Elapsed();
            _timer.Start();
        }

        public void Dispose()
        {
            using (_timer)
            {
                _timer.Stop();
            }
        }

        private void Elapsed()
        {
            if (Tick())
            {
                RaiseEvent();
            }
        }

        protected abstract bool Tick();

    }
}
