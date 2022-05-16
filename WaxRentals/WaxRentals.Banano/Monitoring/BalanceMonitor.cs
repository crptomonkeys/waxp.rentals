using System;
using Nano.Net.Numbers;
using WaxRentals.Banano.Transact;
using WaxRentals.Data.Manager;
using WaxRentals.Monitoring;

namespace WaxRentals.Banano.Monitoring
{
    public class BalanceMonitor : Monitor<BigDecimal>
    {

        private readonly IBananoAccount _account;

        public BalanceMonitor(TimeSpan interval, ILog log, IBananoAccount account)
            : base(interval, log)
        {
            _account = account;
        }

        private BigDecimal _balance;

        protected override bool Tick(out BigDecimal balance)
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
