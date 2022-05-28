namespace WaxRentals.Banano.Transact
{
    public interface IBananoAccountFactory
    {

        IBananoAccount BuildAccount(uint index);
        IBananoAccount BuildWelcomeAccount(uint index);

    }
}
