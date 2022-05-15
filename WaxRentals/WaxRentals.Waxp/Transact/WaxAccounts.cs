using System;
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

        public ITransact Primary { get; }
        public ITransact Today { get { return Transact[DaysPassed % Transact.Length]; } }
        public ITransact Tomorrow { get { return Transact[(DaysPassed + 1) % Transact.Length]; } }

        private ITransact[] Transact { get; }
        private int DaysPassed
        {
            get
            {
                var timespan = DateTime.UtcNow - new DateTime(2022, 05, 15, 0, 0, 0, DateTimeKind.Utc);
                return Convert.ToInt32(timespan.TotalDays);
            }
        }

        public ITransact GetAccount(string account)
        {
            return Transact.Single(transact => string.Equals(account, transact.Account, StringComparison.OrdinalIgnoreCase));
        }

    }
}
