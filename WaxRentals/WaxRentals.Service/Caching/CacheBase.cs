using WaxRentals.Data.Manager;

namespace WaxRentals.Service.Caching
{
    public abstract class CacheBase
    {

        protected IDataFactory Factory { get; }

        public CacheBase(IDataFactory factory)
        {
            Factory = factory;
        }

    }
}
