using WaxRentals.Banano.Config;
using WaxRentals.Data.Manager;

namespace WaxRentals.Banano.Transact
{
    internal class BananoAccountFactory : IBananoAccountFactory
    {

        private readonly BananoSeed _seed;
        private readonly RpcClients _rpc;
        private readonly IDataFactory _factory;

        public BananoAccountFactory(BananoSeed seed, RpcClients rpc, IDataFactory factory)
        {
            _seed = seed;
            _rpc = rpc;
            _factory = factory;
        }

        public IBananoAccount BuildAccount(uint index)
        {
            return new WrappedAccount(_seed, index, _rpc, _factory);
        }

    }
}
