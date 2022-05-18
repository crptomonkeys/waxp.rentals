using WaxRentals.Banano.Config;
using WaxRentals.Data.Manager;

namespace WaxRentals.Banano.Transact
{
    internal class BananoAccountFactory : IBananoAccountFactory
    {

        private readonly BananoSeed _seed;
        private readonly RpcClients _rpc;
        private readonly ILog _log;

        public BananoAccountFactory(BananoSeed seed, RpcClients rpc, ILog log)
        {
            _seed = seed;
            _rpc = rpc;
            _log = log;
        }

        public IBananoAccount BuildAccount(uint index)
        {
            return new WrappedAccount(_seed, index, _rpc, _log);
        }

    }
}
