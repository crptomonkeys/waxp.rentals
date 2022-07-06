using WaxRentals.Data.Manager;

namespace WaxRentals.Service.Caching
{
    public abstract class CacheBase
    {

        protected ILog Log { get; }

        public CacheBase(ILog log)
        {
            Log = log;
        }

    }
}
