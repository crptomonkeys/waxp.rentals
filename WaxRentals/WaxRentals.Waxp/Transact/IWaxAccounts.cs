namespace WaxRentals.Waxp.Transact
{
    public interface IWaxAccounts
    {

        ITransact Primary { get; }
        ITransact Today { get; }
        ITransact Tomorrow { get; }

        ITransact GetAccount(string account);

    }
}
