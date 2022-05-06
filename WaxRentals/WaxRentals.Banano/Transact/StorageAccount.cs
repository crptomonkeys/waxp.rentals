using WaxRentals.Banano.Config;

namespace WaxRentals.Banano.Transact
{
    public class StorageAccount : WrappedAccount
    {

        public StorageAccount(BananoSeed seed, RpcClients rpc) : base (seed, 0, rpc) { }

    }
}
