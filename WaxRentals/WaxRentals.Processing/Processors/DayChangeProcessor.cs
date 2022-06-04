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
            if (Today != today)
            {
                var (todaySuccess, todayBalances) = await today.GetBalances();
                if (todaySuccess && todayBalances.Unstaking > 0)
                {
                    await today.ClaimRefund();
                }

                var (yesterdaySuccess, yesterdayBalances) = await Wax.Yesterday.GetBalances();
                if (yesterdaySuccess && yesterdayBalances.Available > 0)
                {
                    await Wax.Yesterday.Send(today.Account, yesterdayBalances.Available);
                }

                if (todaySuccess && yesterdaySuccess)
                {
                    Today = today;
                }
            }
        }

    }
}
