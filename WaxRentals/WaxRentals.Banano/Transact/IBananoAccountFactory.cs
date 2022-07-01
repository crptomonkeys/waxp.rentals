namespace WaxRentals.Banano.Transact
{
    public interface IBananoAccountFactory
    {

        IBananoAccount BuildAccount(int index);
        IBananoAccount BuildWelcomeAccount(int index);

    }
}
