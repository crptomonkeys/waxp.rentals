using WaxRentals.Data.Manager;
using WaxRentals.Service.Caching.Values;
using WaxRentals.Waxp.Transact;

namespace WaxRentals.Service.Caching
{
    public class WaxInfoCache : InvalidatableCache
    {

        public AccountBalances GetBalances() => new()
        {
            Today = Today,
            Available = Available.Value,
            Unstaking = Unstaking.Value,
            Staked = Staked.Value
        };


        private IWaxAccounts Accounts { get; }
        private VolatileDecimal Available { get; } = new VolatileDecimal();
        private VolatileDecimal Unstaking { get; } = new VolatileDecimal();
        private VolatileDecimal Staked { get; } = new VolatileDecimal();
        private volatile string Today = string.Empty;

        public WaxInfoCache(IDataFactory factory, TimeSpan interval, IWaxAccounts accounts)
            : base(factory, interval)
        {
            Accounts = accounts;
        }

        protected async override Task Tick()
        {
            var tasks = Accounts.Transact.ToDictionary(account => account, account => account.GetBalances());
            var success = (await Task.WhenAll(tasks.Values)).All(result => result.Success);

            if (success)
            {
                var (_, today) = await tasks[Accounts.Today];
                var (_, tomorrow) = await tasks[Accounts.Tomorrow];

                Available.Value = today.Available;
                Unstaking.Value = tomorrow.Unstaking + tomorrow.Available;
                Today = Accounts.Today.Account;

                decimal staked = 0;
                foreach (var kvp in tasks)
                {
                    staked += (await kvp.Value).Balances.Staked;
                }
                Staked.Value = staked;
            }
        }

    }
}
