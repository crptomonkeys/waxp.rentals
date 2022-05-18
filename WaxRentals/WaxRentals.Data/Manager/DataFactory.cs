using System;
using Microsoft.Extensions.DependencyInjection;

namespace WaxRentals.Data.Manager
{
    internal class DataFactory : IDataFactory
    {

        private IServiceProvider _provider;

        public DataFactory(IServiceProvider provider)
        {
            _provider = provider;
        }

        public IInsert   Insert   { get { return _provider.GetRequiredService<DataManager>(); } }
        public ILog      Log      { get { return _provider.GetRequiredService<DataManager>(); } }
        public IProcess  Process  { get { return _provider.GetRequiredService<DataManager>(); } }
        public ITrackWax TrackWax { get { return _provider.GetRequiredService<DataManager>(); } }
        public IWork     Work     { get { return _provider.GetRequiredService<DataManager>(); } }

    }
}
