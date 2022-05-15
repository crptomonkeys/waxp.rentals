using System;
using System.Threading.Tasks;
using WaxRentals.Data.Manager;
using WaxRentals.Waxp.Transact;

namespace WaxRentals.Processing.Processors
{
    internal class DayChangeProcessor : Processor<ITransact>
    {

        private IWaxAccounts Wax { get; }
        private ITransact Today { get; set; }

        public DayChangeProcessor(IProcess data, ILog log, IWaxAccounts wax)
            : base(data, log)
        {
            Wax = wax;
        }

        protected override Func<Task<ITransact>> Get => () => Task.FromResult(Wax.Today);
        protected override async Task Process(ITransact today)
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
                /*
                var available = yesterday.??;
                if (available > 0)
                {
                    await yesterday.Send(today.Account, );
                }
                */
            }
        }

    }
}
