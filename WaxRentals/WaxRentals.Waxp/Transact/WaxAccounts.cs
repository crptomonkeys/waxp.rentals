using System;
using System.Linq;
using Eos.Cryptography;
using WaxRentals.Data.Manager;
using static WaxRentals.Service.Shared.Config.Constants;
using static WaxRentals.Waxp.Config.Constants;

namespace WaxRentals.Waxp.Transact
{
    internal class WaxAccounts : IWaxAccounts
    {

        public WaxAccounts(PrivateKey active, IClientFactory client, IDataFactory factory)
        {
            Primary = new WrappedAccount(Wax.PrimaryAccount, active, client, factory);
            Transact = Protocol.TransactAccounts.Select(
                account => new WrappedAccount(account, active, client, factory)
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
