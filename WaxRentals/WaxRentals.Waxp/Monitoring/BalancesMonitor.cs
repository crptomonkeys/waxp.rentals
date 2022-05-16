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

        public BalancesMonitor(TimeSpan interval, ILog log, IWaxAccounts wax)
            : base(interval, log)
        {
            _wax = wax;
        }

        private AccountBalances _balances = new();
        
        protected override bool Tick(out AccountBalances balances)
        {
            balances = GetBalances().GetAwaiter().GetResult();
            if (_balances.Available != balances.Available ||
                _balances.Staked != balances.Staked ||
                _balances.Unstaking != balances.Unstaking)
            {
                _balances = balances;
                return true;
            }
            return false;
        }

        private async Task<AccountBalances> GetBalances()
        {
            var tasks = _wax.Transact.ToDictionary(account => account, account => account.GetBalances());
            await Task.WhenAll(tasks.Values);

            var balances = new AccountBalances();
            balances.Available = (await tasks[_wax.Today]).Available;
            balances.Unstaking = (await tasks[_wax.Tomorrow]).Unstaking;
            foreach (var kvp in tasks)
            {
                balances.Staked += (await kvp.Value).Staked;
            }
            return balances;
        }

    }
}
