using N2.Pow;
using WaxRentals.Banano.Config;
using WaxRentals.Data.Manager;

namespace WaxRentals.Banano.Transact
{
    internal class AdsAccount : WrappedAccount, IAdsAccount
    {

        public AdsAccount(BananoSeed seed, RpcClients rpc, WorkServer workServer, ILog log) : base (seed, 0, rpc, workServer, log) { }

    }
}
