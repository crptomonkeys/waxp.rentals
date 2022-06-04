using System;
using System.Linq;
using System.Threading.Tasks;
using WaxRentals.Data.Manager;
using WaxRentals.Monitoring;
using WaxRentals.Waxp.Transact;

namespace WaxRentals.Waxp.Monitoring
{
    public class BalancesMonitor : Monitor<AccountBalances>
    {

        private readonly IWaxAccounts _wax;

        public BalancesMonitor(TimeSpan interval, IDataFactory factory, IWaxAccounts wax)
            : base(interval, factory)
        {
            _wax = wax;
        }

        private AccountBalances _balances = new();
        
        protected override bool Tick(out AccountBalances balances)
        {
            var result = GetBalances().GetAwaiter().GetResult();
            balances = result.Balances;

            if (result.Success)
            {
                if (_balances.Available != balances.Available ||
                    _balances.Staked    != balances.Staked    ||
                    _balances.Unstaking != balances.Unstaking ||
                    !string.Equals(_balances.Today, balances.Today, StringComparison.OrdinalIgnoreCase))
                {
                    _balances = balances;
                    return true;
                }
            }
            return false;
        }

        private async Task<(bool Success, AccountBalances Balances)> GetBalances()
        {
            var tasks = _wax.Transact.ToDictionary(account => account, account => account.GetBalances());
            var success = (await Task.WhenAll(tasks.Values)).All(result => result.Success);

            if (success)
            {
                var (_, today) = await tasks[_wax.Today];
                var (_, tomorrow) = await tasks[_wax.Tomorrow];

                var balances = new AccountBalances();
                balances.Available = today.Available;
                balances.Unstaking = tomorrow.Unstaking + tomorrow.Available;
                foreach (var kvp in tasks)
                {
                    balances.Staked += (await kvp.Value).Balances.Staked;
                }
                balances.Today = _wax.Today.Account;
                return (true, balances);
            }
            return (false, null);
        }

    }
}
