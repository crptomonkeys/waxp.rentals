using System;
using System.Threading.Tasks;
using System.Timers;
using WaxRentals.Data.Manager;

namespace WaxRentals.Processing.Processors
{
    internal abstract class Processor<T>
    {

        protected IProcess Data { get; }
        protected ILog Log { get; }

        protected Processor(IProcess data, ILog log)
        {
            Data = data;
            Log = log;
        }
        
        #region " Processing "

        private Timer _timer;

        public void Start(TimeSpan interval)
        {
            if (_timer == null)
            {
                _timer = new Timer(interval.TotalMilliseconds);
                _timer.Elapsed += async (_, _) => await Tick();
                _timer.Start();
            }
        }

        private bool _running;
        private readonly object _locker = new object();
        private async Task Tick()
        {
            if (!_running)
            {
                bool run = false;
                lock (_locker)
                {
                    if (!_running)
                    {
                        _running = true;
                        run = true;
                    }
                }

                if (run)
                {
                    try
                    {
                        await Run();
                    }
                    finally
                    {
                        lock (_locker)
                        {
                            _running = false;
                        }
                    }
                }
            }
        }

        private async Task Run()
        {
            T target = default;
            try
            {
                // Process one at a time.
                // Revisit if this ends up being too slow.
                target = await Get();
                while (target != null)
                {
                    await Process(target);
                    target = await Get();
                }
            }
            catch (Exception ex)
            {
                await Log.Error(ex, context: target);
            }
        }

        protected abstract Func<Task<T>> Get { get; }
        protected abstract Task Process(T target);

        #endregion

    }
}
