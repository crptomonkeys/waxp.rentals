using System;
using System.Threading.Tasks;
using WaxRentals.Data.Manager;
using WaxRentals.Waxp.Transact;

namespace WaxRentals.Processing.Processors
{
    internal class DayChangeProcessor : Processor<IWaxAccount>
    {

        private IWaxAccounts Wax { get; }
        private IWaxAccount Today { get; set; }

        public DayChangeProcessor(IDataFactory factory, IWaxAccounts wax)
            : base(factory)
        {
            Wax = wax;
        }

        protected override Func<Task<IWaxAccount>> Get => () => Task.FromResult(Wax.Today);
        protected async override Task Process(IWaxAccount today)
        {
            if (Today == null)
            {
                Today = today;
            }
            else if (Today != today)
            {
                var yesterday = Today;
                Today = today;

                await today.ClaimRefund();
                
                var available = (await yesterday.GetBalances()).Available;
                if (available > 0)
                {
                    await yesterday.Send(today.Account, available);
                }
            }
        }

    }
}
