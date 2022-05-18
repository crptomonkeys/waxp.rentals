﻿using System;
using System.Linq;
using Eos.Cryptography;
using WaxRentals.Data.Manager;
using static WaxRentals.Waxp.Config.Constants;

namespace WaxRentals.Waxp.Transact
{
    internal class WaxAccounts : IWaxAccounts
    {

        public WaxAccounts(PrivateKey active, IClientFactory client, IDataFactory factory)
        {
            Primary = new WrappedAccount(Protocol.Account, active, client, factory);
            Transact = Protocol.TransactAccounts.Select(
                account => new WrappedAccount(account, active, client, factory)
            ).ToArray();
        }

        public IWaxAccount Primary { get; }
        public IWaxAccount Yesterday { get { return Transact[(DaysPassed - 1) % Transact.Length]; } }
        public IWaxAccount Today { get { return Transact[DaysPassed % Transact.Length]; } }
        public IWaxAccount Tomorrow { get { return Transact[(DaysPassed + 1) % Transact.Length]; } }

        public IWaxAccount[] Transact { get; }

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

    }
}
