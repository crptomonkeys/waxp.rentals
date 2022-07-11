using System;
using System.Threading.Tasks;
using System.Timers;
using WaxRentals.Data.Manager;

namespace WaxRentals.Monitoring
{
    public abstract class Monitor : IDisposable
    {

        #region " Event "

        public event EventHandler Updated;
        protected ILog Log { get; }

        protected async Task RaiseEvent()
        {
            try
            {
                Updated?.Invoke(this, EventArgs.Empty);
            }
            catch (Exception ex)
            {
                await Log.Error(ex);
            }
        }

        public async Task Initialize()
        {
            await Elapsed();
            await RaiseEvent();
        }

        #endregion

        #region " Timer "

        private readonly Timer _timer;

        protected Monitor(TimeSpan interval, ILog log)
        {
            Log = log;

            _timer = new Timer(interval.TotalMilliseconds);
            _timer.Elapsed += async (_, _) => await Elapsed();
            _timer.Start();
        }

        public void Dispose()
        {
            _timer.Elapsed -= async (_, _) => await Elapsed();
            using (_timer)
            {
                _timer.Stop();
            }
            GC.SuppressFinalize(this);
        }

        protected async virtual Task Elapsed()
        {
            try
            {
                if (await Tick())
                {
                    await RaiseEvent();
                }
            }
            catch (Exception ex)
            {
                await Log.Error(ex);
            }
        }

        protected abstract Task<bool> Tick();

        #endregion

    }
}
