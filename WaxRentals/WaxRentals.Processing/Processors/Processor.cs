﻿using System;
using System.Threading.Tasks;
using System.Timers;
using Microsoft.Extensions.DependencyInjection;
using WaxRentals.Service.Shared.Connectors;
using WaxRentals.Service.Shared.Entities.Input;
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

        private ITrackService Track { get; }
        protected virtual bool ProcessMultiplePerTick { get; } = true;

        protected Processor(ITrackService track)
        {
            Track = track;
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
                Log(ex, context: target);
            }
            _complete.Set();
        }

        protected abstract Func<Task<T>> Get { get; }
        protected abstract Task Process(T target);

        #endregion

        #region " Tracking "

        protected async void Log(Exception ex, string error = null, object context = null)
        {
            var log = new ErrorLog { Exception = ex, Error = error, Context = context };
            await Track.Error(log);
        }

        protected async void LogTransaction(string description, decimal quantity, string coin, decimal? earned = null, decimal? spent = null)
        {
            var log = new TransactionLog { Description = description, Quantity = quantity, Coin = coin, Earned = earned, Spent = spent };
            await Track.Transaction(log);
        }

        protected async void Notify(string message)
        {
            await Track.Notify(message);
        }

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
