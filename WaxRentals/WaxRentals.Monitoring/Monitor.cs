using System;
using System.Timers;

namespace WaxRentals.Monitoring
{
    public abstract class Monitor : IDisposable
    {

        #region " Event "

        public event EventHandler Updated;

        protected void RaiseEvent()
        {
            try
            {
                Updated?.Invoke(this, EventArgs.Empty);
            }
            catch (Exception ex)
            {
                // Log errors.
            }
        }

        public void Initialize()
        {
            Elapsed();
        }

        #endregion

        #region " Timer "

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

        protected virtual void Elapsed()
        {
            if (Tick())
            {
                RaiseEvent();
            }
        }

        protected abstract bool Tick();

        #endregion

    }

    public abstract class Monitor<T> : Monitor
    {

        #region " Event "

        protected Monitor(TimeSpan interval) : base(interval) { }

        public new event EventHandler<T> Updated;

        protected void RaiseEvent(T result)
        {
            try
            {
                Updated?.Invoke(this, result);
                base.RaiseEvent();
            }
            catch (Exception ex)
            {
                // Log errors.
            }
        }

        #endregion

        #region " Timer "

        protected override void Elapsed()
        {
            if (Tick(out T args))
            {
                RaiseEvent(args);
            }
        }

        protected abstract bool Tick(out T result);

        protected override bool Tick()
        {
            // This should never be called.
            return false;
        }

        #endregion

    }
}
