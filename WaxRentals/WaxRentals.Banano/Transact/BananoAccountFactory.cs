using WaxRentals.Banano.Config;
using WaxRentals.Data.Manager;

namespace WaxRentals.Banano.Transact
{
    internal class BananoAccountFactory : IBananoAccountFactory
    {

        private readonly BananoSeed _seed;
        private readonly BananoSeed _welcomeSeed;
        private readonly RpcClients _rpc;
        private readonly IDataFactory _factory;

        public BananoAccountFactory(BananoSeed seed, BananoSeed welcomeSeed, RpcClients rpc, IDataFactory factory)
        {
            _seed = seed;
            _welcomeSeed = welcomeSeed;
            _rpc = rpc;
            _factory = factory;
        }

        public IBananoAccount BuildAccount(int index)
        {
            return new WrappedAccount(_seed, (uint)index, _rpc, _factory);
        }

        public IBananoAccount BuildWelcomeAccount(int index)
        {
            return new WrappedAccount(_welcomeSeed, (uint)index, _rpc, _factory);
        }

    }
}
