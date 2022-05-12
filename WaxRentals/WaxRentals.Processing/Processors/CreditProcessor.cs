using System;
using System.Threading.Tasks;
using WaxRentals.Data.Entities;
using WaxRentals.Data.Manager;
using WaxRentals.Waxp.Transact;
using static WaxRentals.Processing.Constants;

namespace WaxRentals.Processing.Processors
{
    internal class CreditProcessor : Processor
    {

        private readonly IProcess _data;
        private readonly ITransact _wax;

        public CreditProcessor(IProcess data, ITransact wax)
        {
            _data = data;
            _wax = wax;
        }

        protected override async Task Run()
        {
            // Process credits one at a time.
            // Revisit if this ends up being too slow.
            var credit = await _data.PullNextCredit();
            while (credit != null)
            {
                await Process(credit);
                credit = await _data.PullNextCredit();
            }
        }

        private async Task Process(Credit credit)
        {
            var account = credit.Account;

            var wax = account.CPU + account.NET;
            var bananoPerDay = wax * Calculations.BananoPerWaxPerDay;
            var days = Convert.ToDouble(credit.Banano / bananoPerDay);

            if (account.PaidThrough.HasValue)
            {
                await _data.ProcessCredit(credit.CreditId, account.PaidThrough.Value.AddDays(days));
            }
            else
            {
                // We're opening the account, so we have to stake the CPU and NET.
                var success = await _wax.Stake(account.WaxAccount, account.CPU, account.NET);
                if (success)
                {
                    await _data.ProcessCredit(credit.CreditId, DateTime.UtcNow.AddDays(days));
                }
            }
        }

    }
}
