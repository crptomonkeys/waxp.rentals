using System.Collections.Concurrent;
using WaxRentals.Data.Manager;
using WaxRentals.Waxp.Transact;

#nullable disable

namespace WaxRentals.Service.Caching
{
    public class WaxInfoCache : InvalidatableCache
    {

        public AccountBalances GetBalances()
        {
            var today = GetBalances(Accounts.Today.Account);
            var tomorrow = GetBalances(Accounts.Tomorrow.Account);
            return new AccountBalances
            {
                Account = Accounts.Today.Account,
                Available = (today?.Available).GetValueOrDefault(),
                Unstaking = (tomorrow?.Unstaking).GetValueOrDefault() + (tomorrow?.Available).GetValueOrDefault(),
                Staked = Accounts.Transact.Sum(account => (GetBalances(account.Account)?.Staked).GetValueOrDefault())
            };
        }

        public AccountBalances GetBalances(string account)
        {
            return Balances.ContainsKey(account) ? Balances[account] : null;
        }


        private IWaxAccounts Accounts { get; }
        private ConcurrentDictionary<string, AccountBalances> Balances { get; } = new ConcurrentDictionary<string, AccountBalances>(StringComparer.OrdinalIgnoreCase);
        
        public WaxInfoCache(IDataFactory factory, TimeSpan interval, IWaxAccounts accounts)
            : base(factory, interval)
        {
            Accounts = accounts;
        }

        protected async override Task Tick()
        {
            var tasks = Accounts.Transact.ToDictionary(account => account.Account, account => account.GetBalances());
            tasks[Accounts.Primary.Account] = Accounts.Primary.GetBalances();
            var success = (await Task.WhenAll(tasks.Values)).All(result => result.Success);

            if (success)
            {
                foreach (var kvp in tasks)
                {
                    Balances[kvp.Key] = (await kvp.Value).Balances;
                }
            }
        }

    }
}
