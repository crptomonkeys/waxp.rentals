using System;
using System.Threading.Tasks;
using WaxRentals.Banano.Transact;
using WaxRentals.Data.Manager;

namespace WaxRentals.Processing.Processors
{
    internal class WorkProcessor : Processor<int?>
    {

        private IBananoAccountFactory Banano { get; }

        public WorkProcessor(IDataFactory factory, IBananoAccountFactory banano)
            : base(factory)
        {
            Banano = banano;
        }

        protected override Func<Task<int?>> Get => Factory.Work.PullNextAddress;
        protected async override Task Process(int? addressId)
        {
            var work = await Banano.BuildAccount((uint)addressId).GenerateWork();
            await Factory.Work.SaveWork(addressId.Value, work);
        }

    }
}
