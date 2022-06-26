using WaxRentals.Data.Manager;

namespace WaxRentals.Service.Caching
{
    public abstract class InvalidatableCache : TimedCacheBase
    {

        public InvalidatableCache(IDataFactory factory, TimeSpan interval)
            : base(factory, interval)
        {

        }

        public async Task Invalidate()
        {
            await Elapsed();
        }

    }
}
