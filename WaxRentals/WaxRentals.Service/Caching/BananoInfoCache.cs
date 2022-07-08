using WaxRentals.Banano.Transact;
using WaxRentals.Data.Manager;
using WaxRentals.Service.Caching.Values;

namespace WaxRentals.Service.Caching
{
    public class BananoInfoCache : InvalidatableCache
    {

        public decimal GetBalance() => Balance.Value;

        private IBananoAccount Account { get; }
        private LockedDecimal Balance { get; } = new LockedDecimal();

        public BananoInfoCache(ILog log, TimeSpan interval, IBananoAccount account)
            : base(log, interval)
        {
            Account = account;
        }

        protected async override Task Tick()
        {
            Balance.Value = await Account.GetBalance();
        }

    }
}
