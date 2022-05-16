namespace WaxRentals.Banano.Transact
{
    public interface IBananoAccountFactory
    {

        IBananoAccount BuildAccount(uint index);

    }
}
