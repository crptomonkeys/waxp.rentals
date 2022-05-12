using WaxRentals.Banano.Config;

namespace WaxRentals.Banano.Transact
{
    internal class BananoAccountFactory : IBananoAccountFactory
    {

        private readonly BananoSeed _seed;
        private readonly RpcClients _rpc;

        public BananoAccountFactory(BananoSeed seed, RpcClients rpc)
        {
            _seed = seed;
            _rpc = rpc;
        }

        public ITransact BuildAccount(uint index)
        {
            return new WrappedAccount(_seed, index, _rpc);
        }

    }
}
