using System;
using System.Threading.Tasks;
using System.Timers;

namespace WaxRentals.Processing.Processors
{
    internal abstract class Processor
    {

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

        protected abstract Task Run();

    }
}
