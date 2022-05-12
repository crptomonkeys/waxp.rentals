using WaxRentals.Banano.Config;
using WaxRentals.Data.Manager;

namespace WaxRentals.Banano.Transact
{
    internal class StorageAccount : WrappedAccount
    {

        public StorageAccount(BananoSeed seed, RpcClients rpc, IInsert data, ILog log) : base (seed, 0, rpc, data, log) { }

    }
}
