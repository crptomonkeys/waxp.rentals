using System;
using Nano.Net.Numbers;
using WaxRentals.Banano.Transact;
using WaxRentals.Data.Manager;
using WaxRentals.Monitoring;
using static WaxRentals.Banano.Config.Constants;

namespace WaxRentals.Banano.Monitoring
{
    public class ReceiveMonitor : Monitor<BigDecimal>
    {

        private readonly IBananoAccount _account;

        public ReceiveMonitor(TimeSpan interval, ILog log, IBananoAccount account)
            : base(interval, log)
        {
            _account = account;
        }

        protected override bool Tick(out BigDecimal received)
        {
            received = _account.Receive(verifyOnly: false).GetAwaiter().GetResult();
            received = received / Math.Pow(10, Protocol.Decimals);
            return received > 0;
        }

    }
}
