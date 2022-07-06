using WaxRentals.Data.Manager;
using Timer = System.Timers.Timer;

namespace WaxRentals.Service.Caching
{
    public abstract class TimedCacheBase : CacheBase, IDisposable
    {

        private Timer Timer { get; }

        public TimedCacheBase(ILog log, TimeSpan interval)
            : base(log)
        {
            Timer = new Timer(interval.TotalMilliseconds);
            Timer.Elapsed += async (_, _) => await Elapsed();
            Timer.Start();
            Task.Delay(1).ContinueWith(task => Elapsed()); // Kick off immediately (but let base classes finish constructing first).
        }

        public void Dispose()
        {
            Timer.Elapsed -= async (_, _) => await Elapsed();
            using (Timer)
            {
                Timer.Stop();
            }
        }

        protected async Task Elapsed()
        {
            try
            {
                await Tick();
            }
            catch (Exception ex)
            {
                await Log.Error(ex);
            }
        }

        protected abstract Task Tick();

    }
}
