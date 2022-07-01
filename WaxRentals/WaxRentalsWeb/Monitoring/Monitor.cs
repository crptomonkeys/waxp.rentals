using System;
using System.Timers;
using WaxRentals.Data.Manager;

namespace WaxRentalsWeb.Monitoring
{
    public abstract class Monitor : IDisposable
    {

        #region " Event "

        public event EventHandler Updated;
        protected IDataFactory Factory { get; }

        protected void RaiseEvent()
        {
            try
            {
                Updated?.Invoke(this, EventArgs.Empty);
            }
            catch (Exception ex)
            {
                Factory.Log.Error(ex);
            }
        }

        public void Initialize()
        {
            Elapsed();
            RaiseEvent();
        }

        #endregion

        #region " Timer "

        private readonly Timer _timer;

        protected Monitor(TimeSpan interval, IDataFactory factory)
        {
            Factory = factory;

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
            try
            {
                if (Tick())
                {
                    RaiseEvent();
                }
            }
            catch (Exception ex)
            {
                Factory.Log.Error(ex);
            }
        }

        protected abstract bool Tick();

        #endregion

    }

    public abstract class Monitor<T> : Monitor
    {

        #region " Event "

        protected Monitor(TimeSpan interval, IDataFactory factory) : base(interval, factory) { }

        public new event EventHandler<T> Updated;

        protected void RaiseEvent(T result)
        {
            try
            {
                Updated?.Invoke(this, result);
                RaiseEvent();
            }
            catch (Exception ex)
            {
                Factory.Log.Error(ex);
            }
        }

        #endregion

        #region " Timer "

        protected override void Elapsed()
        {
            try
            {
                if (Tick(out T args))
                {
                    RaiseEvent(args);
                }
            }
            catch (Exception ex)
            {
                Factory.Log.Error(ex);
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
