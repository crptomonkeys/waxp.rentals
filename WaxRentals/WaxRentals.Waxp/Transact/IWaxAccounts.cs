using System.Collections.Generic;

namespace WaxRentals.Waxp.Transact
{
    public interface IWaxAccounts
    {

        IWaxAccount Primary { get; }
        IWaxAccount Today { get; }
        IWaxAccount Tomorrow { get; }

        IWaxAccount[] Transact { get; }

        IWaxAccount GetAccount(string account);

    }
}
