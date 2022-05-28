using System;
using System.Threading.Tasks;
using System.Timers;
using Microsoft.Extensions.DependencyInjection;
using WaxRentals.Data.Manager;
using ManualResetEventSlim = System.Threading.ManualResetEventSlim;

namespace WaxRentals.Processing.Processors
{
    internal interface IProcessor
    {
        void Start(TimeSpan interval);
        ManualResetEventSlim Stop();
    }

    internal abstract class Processor<T> : IProcessor
    {
        private readonly ManualResetEventSlim _complete = new();

        protected IDataFactory Factory { get; }
        protected virtual bool ProcessMultiplePerTick { get; } = true;

        protected Processor(IDataFactory factory)
        {
            Factory = factory;
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

        public ManualResetEventSlim Stop()
        {
            if (_timer != null)
            {
                _timer.Stop();
            }
            if (!_running)
            {
                _complete.Set();
            }
            return _complete;
        }

        private bool _running;
        private readonly object _locker = new();
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
            _complete.Reset();
            T target = default;
            try
            {
                // Process one at a time.
                // Revisit if this ends up being too slow.
                target = await Get();
                if (ProcessMultiplePerTick)
                {
                    while (target != null)
                    {
                        await Process(target);
                        target = await Get();
                    }
                }
                else if (target != null)
                {
                    await Process(target);
                }
            }
            catch (Exception ex)
            {
                await Factory.Log.Error(ex, context: target);
            }
            _complete.Set();
        }

        protected abstract Func<Task<T>> Get { get; }
        protected abstract Task Process(T target);

        #endregion

    }

    internal static class IServiceProviderExtensions
    {
        public static T BuildProcessor<T>(this IServiceProvider @this, TimeSpan interval)
            where T : IProcessor
        {
            var processor = @this.GetRequiredService<T>();
            processor.Start(interval);
            return processor;
        }
    }
}
