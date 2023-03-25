using N2.Pow;
using WaxRentals.Banano.Config;
using WaxRentals.Data.Manager;

namespace WaxRentals.Banano.Transact
{
    internal class BananoAccountFactory : IBananoAccountFactory
    {

        private readonly BananoSeed _seed;
        private readonly BananoSeed _welcomeSeed;
        private readonly RpcClients _rpc;
        private readonly WorkServer _workServer;
        private readonly ILog _log;

        public BananoAccountFactory(BananoSeed seed, BananoSeed welcomeSeed, RpcClients rpc, WorkServer workServer, ILog log)
        {
            _seed = seed;
            _welcomeSeed = welcomeSeed;
            _rpc = rpc;
            _workServer = workServer;
            _log = log;
        }

        public IBananoAccount BuildAccount(int index)
        {
            return new WrappedAccount(_seed, (uint)index, _rpc, _workServer, _log);
        }

        public IBananoAccount BuildWelcomeAccount(int index)
        {
            return new WrappedAccount(_welcomeSeed, (uint)index, _rpc, _workServer, _log);
        }

    }
}
