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
        private readonly ITransact _storage;

        public AccountMonitor(TimeSpan interval, ITransact account, ITransact storage = null)
            : base(interval)
        {
            _account = account;
            _storage = storage;
        }

        protected override bool Tick(out BigDecimal received)
        {
            received = _account.Receive().GetAwaiter().GetResult();
            received = received / Math.Pow(10, Protocol.Decimals);
            if (received > 0)
            {
                if (_storage != null)
                {
                    _account.Send(_storage.Address, received);
                }
                return true;
            }
            return false;
        }

    }
}
