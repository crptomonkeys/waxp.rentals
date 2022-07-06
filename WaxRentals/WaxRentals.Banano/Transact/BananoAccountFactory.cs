using WaxRentals.Banano.Config;
using WaxRentals.Data.Manager;

namespace WaxRentals.Banano.Transact
{
    internal class BananoAccountFactory : IBananoAccountFactory
    {

        private readonly BananoSeed _seed;
        private readonly BananoSeed _welcomeSeed;
        private readonly RpcClients _rpc;
        private readonly ILog _log;

        public BananoAccountFactory(BananoSeed seed, BananoSeed welcomeSeed, RpcClients rpc, ILog log)
        {
            _seed = seed;
            _welcomeSeed = welcomeSeed;
            _rpc = rpc;
            _log = log;
        }

        public IBananoAccount BuildAccount(int index)
        {
            return new WrappedAccount(_seed, (uint)index, _rpc, _log);
        }

        public IBananoAccount BuildWelcomeAccount(int index)
        {
            return new WrappedAccount(_welcomeSeed, (uint)index, _rpc, _log);
        }

    }
}
