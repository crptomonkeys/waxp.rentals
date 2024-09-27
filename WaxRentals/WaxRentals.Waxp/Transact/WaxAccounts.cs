using System;
using System.Linq;
using Eos.Cryptography;
using WaxRentals.Data.Manager;

namespace WaxRentals.Waxp.Transact
{
    internal class WaxAccounts : IWaxAccounts
    {

        public WaxAccounts(AccountNames names, PrivateKey active, IClientFactory client, ILog log)
        {
            Primary = new WrappedAccount(names.Primary, active, client, log);
            Transact = names.Transact.Select(
                account => new WrappedAccount(account, active, client, log)
            ).ToArray();
        }

        public IWaxAccount Primary { get; }
        public IWaxAccount Yesterday { get { return GetAccount(-1); } }
        public IWaxAccount Today { get { return GetAccount(0); } }
        public IWaxAccount Tomorrow { get { return GetAccount(1); } }

        public IWaxAccount[] Transact { get; }

        private static readonly DateTime _startDate = new(2022, 05, 15, 0, 0, 0, DateTimeKind.Utc);
        private static int DaysPassed
        {
            get
            {
                var timespan = DateTime.UtcNow - _startDate;
                return Convert.ToInt32(timespan.TotalDays);
            }
        }

        public IWaxAccount GetAccount(int daysOffset)
        {
            return Transact[(DaysPassed + daysOffset) % Transact.Length];
        }

        public IWaxAccount GetAccount(string account)
        {
            return Transact.Single(transact => string.Equals(account, transact.Account, StringComparison.OrdinalIgnoreCase));
        }

    }
}
