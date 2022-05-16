using System;
using System.Collections.Generic;
using System.Linq;
using Eos.Cryptography;
using static WaxRentals.Waxp.Config.Constants;

namespace WaxRentals.Waxp.Transact
{
    internal class WaxAccounts : IWaxAccounts
    {

        public WaxAccounts(PrivateKey active, ClientFactory client)
        {
            Primary = new WrappedAccount(Protocol.Account, active, client);
            Transact = Protocol.TransactAccounts.Select(
                account => new WrappedAccount(account, active, client)
            ).ToArray();
        }

        public IWaxAccount Primary { get; }
        public IWaxAccount Today { get { return Transact[DaysPassed % Transact.Length]; } }
        public IWaxAccount Tomorrow { get { return Transact[(DaysPassed + 1) % Transact.Length]; } }

        private IWaxAccount[] Transact { get; }

        private static readonly DateTime _startDate = new DateTime(2022, 05, 15, 0, 0, 0, DateTimeKind.Utc);
        private int DaysPassed
        {
            get
            {
                var timespan = DateTime.UtcNow - _startDate;
                return Convert.ToInt32(timespan.TotalDays);
            }
        }

        public IWaxAccount GetAccount(string account)
        {
            return Transact.Single(transact => string.Equals(account, transact.Account, StringComparison.OrdinalIgnoreCase));
        }

        public IEnumerable<IWaxAccount> GetAllAccounts()
        {
            return Transact.Append(Primary);
        }

    }
}
