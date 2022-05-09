using System;
using Nano.Net.Numbers;
using WaxRentals.Banano.Transact;
using WaxRentals.Monitoring;
using static WaxRentals.Banano.Config.Constants;

namespace WaxRentals.Banano.Monitoring
{
    public class AccountMonitor : Monitor<BigDecimal>
    {

        private readonly ITransact _account;

        public AccountMonitor(TimeSpan interval, ITransact account)
            : base(interval)
        {
            _account = account;
        }

        protected override bool Tick(out BigDecimal received)
        {
            received = _account.Receive().GetAwaiter().GetResult();
            received = received / Math.Pow(10, Protocol.Decimals);
            return received > 0;
        }

    }
}
