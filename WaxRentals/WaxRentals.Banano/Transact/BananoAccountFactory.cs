using WaxRentals.Banano.Config;
using WaxRentals.Data.Manager;

namespace WaxRentals.Banano.Transact
{
    internal class BananoAccountFactory : IBananoAccountFactory
    {

        private readonly BananoSeed _seed;
        private readonly RpcClients _rpc;
        private readonly IInsert _data;
        private readonly ILog _log;

        public BananoAccountFactory(BananoSeed seed, RpcClients rpc, IInsert data, ILog log)
        {
            _seed = seed;
            _rpc = rpc;
            _data = data;
            _log = log;
        }

        public ITransact BuildAccount(uint index)
        {
            return new WrappedAccount(_seed, index, _rpc, _data, _log);
        }

    }
}
