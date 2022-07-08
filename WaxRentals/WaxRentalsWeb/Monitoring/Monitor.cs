using System;
using System.Threading.Tasks;
using System.Timers;
using WaxRentals.Service.Shared.Connectors;

namespace WaxRentalsWeb.Monitoring
{
    public abstract class Monitor : IDisposable
    {

        #region " Event "

        public event EventHandler Updated;
        protected ITrackService Log { get; }

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

        protected Monitor(TimeSpan interval, ITrackService log)
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

        protected virtual async Task Elapsed()
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

    public abstract class Monitor<T> : Monitor
    {

        #region " Event "

        protected Monitor(TimeSpan interval, ITrackService log) : base(interval, log) { }

        public new event EventHandler<T> Updated;

        protected async Task RaiseEvent(T result)
        {
            try
            {
                Updated?.Invoke(this, result);
                await RaiseEvent();
            }
            catch (Exception ex)
            {
                await Log.Error(ex);
            }
        }

        #endregion

        #region " Timer "

        protected async override Task Elapsed()
        {
            try
            {
                if (await Tick(out T args))
                {
                    await RaiseEvent(args);
                }
            }
            catch (Exception ex)
            {
                await Log.Error(ex);
            }
        }

        protected abstract Task<bool> Tick(out T result);

        protected override Task<bool> Tick()
        {
            // This should never be called.
            return Task.FromResult(false);
        }

        #endregion

    }
}
