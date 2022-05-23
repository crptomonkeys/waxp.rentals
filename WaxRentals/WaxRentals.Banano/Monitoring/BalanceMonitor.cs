using System;
using Nano.Net.Numbers;
using WaxRentals.Banano.Transact;
using WaxRentals.Data.Manager;
using WaxRentals.Monitoring;

namespace WaxRentals.Banano.Monitoring
{
    public class BalanceMonitor : Monitor<decimal>
    {

        private readonly IBananoAccount _account;

        public BalanceMonitor(TimeSpan interval, IDataFactory factory, IBananoAccount account)
            : base(interval, factory)
        {
            _account = account;
        }

        private decimal _balance;

        protected override bool Tick(out decimal balance)
        {
            balance = _account.GetBalance().GetAwaiter().GetResult();
            if (_balance != balance)
            {
                _balance = balance;
                return true;
            }
            return false;
        }

    }
}
