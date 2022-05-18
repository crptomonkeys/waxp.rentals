using WaxRentals.Banano.Config;
using WaxRentals.Data.Manager;

namespace WaxRentals.Banano.Transact
{
    internal class StorageAccount : WrappedAccount
    {

        public StorageAccount(BananoSeed seed, RpcClients rpc, IDataFactory factory) : base (seed, 0, rpc, factory) { }

    }
}
