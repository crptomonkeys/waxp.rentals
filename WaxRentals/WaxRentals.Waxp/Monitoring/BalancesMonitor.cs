using System;
using WaxRentals.Data.Manager;
using WaxRentals.Monitoring;
using WaxRentals.Waxp.Transact;

namespace WaxRentals.Waxp.Monitoring
{
    internal class BalancesMonitor : Monitor<AccountBalances>
    {

        private readonly string _account;
        private readonly ClientFactory _client;

        public BalancesMonitor(TimeSpan interval, ILog log, string account, ClientFactory client)
            : base(interval, log)
        {
            _account = account;
            _client = client;
        }

        private AccountBalances _balances = new();
        
        protected override bool Tick(out AccountBalances balances)
        {
            balances = GetBalances();
            if (_balances.Available != balances.Available ||
                _balances.Staked != balances.Staked ||
                _balances.Unstaking != balances.Unstaking)
            {
                _balances = balances;
                return true;
            }
            return false;
        }

        private AccountBalances GetBalances()
        {
            return null;
        }

    }
}
