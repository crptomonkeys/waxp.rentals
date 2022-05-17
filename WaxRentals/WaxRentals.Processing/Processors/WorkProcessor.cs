using System;
using System.Threading.Tasks;
using WaxRentals.Banano.Transact;
using WaxRentals.Data.Manager;

namespace WaxRentals.Processing.Processors
{
    internal class WorkProcessor : Processor<int>
    {

        private IWork Work { get; }
        private IBananoAccountFactory Banano { get; }

        public WorkProcessor(IProcess data, ILog log, IWork work, IBananoAccountFactory banano)
            : base(data, log)
        {
            Work = work;
            Banano = banano;
        }

        protected override Func<Task<int>> Get => Work.PullNextAddress;
        protected override async Task Process(int addressId)
        {
            var work = await Banano.BuildAccount((uint)addressId).GenerateWork();
            await Work.SaveWork(addressId, work);
        }

    }
}
