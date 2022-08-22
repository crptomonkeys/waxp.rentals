using WaxRentals.Banano.Config;
using WaxRentals.Data.Manager;

namespace WaxRentals.Banano.Transact
{
    internal class StorageAccount : WrappedAccount, IStorageAccount
    {

        public StorageAccount(BananoSeed seed, RpcClients rpc, ILog log) : base (seed, 0, rpc, log) { }

    }
}
