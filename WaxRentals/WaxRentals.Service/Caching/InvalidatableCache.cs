using WaxRentals.Data.Manager;

namespace WaxRentals.Service.Caching
{
    public abstract class InvalidatableCache : TimedCacheBase
    {

        public InvalidatableCache(ILog log, TimeSpan interval)
            : base(log, interval)
        {

        }

        public async Task Invalidate()
        {
            await Elapsed();
        }

    }
}
