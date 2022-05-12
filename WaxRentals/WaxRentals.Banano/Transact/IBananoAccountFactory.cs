namespace WaxRentals.Banano.Transact
{
    public interface IBananoAccountFactory
    {

        ITransact BuildAccount(uint index);

    }
}
