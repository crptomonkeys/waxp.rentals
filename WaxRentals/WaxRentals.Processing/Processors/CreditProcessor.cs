using System;
using System.Threading.Tasks;
using WaxRentals.Banano.Transact;
using WaxRentals.Data.Entities;
using WaxRentals.Data.Manager;
using static WaxRentals.Banano.Config.Constants;
using static WaxRentals.Data.Config.Constants;
using WaxAccount = WaxRentals.Waxp.Transact.ITransact;

namespace WaxRentals.Processing.Processors
{
    internal class CreditProcessor : Processor<Credit>
    {

        private WaxAccount Wax { get; }
        private IBananoAccountFactory Banano { get; }

        public CreditProcessor(IProcess data, ILog log, WaxAccount wax, IBananoAccountFactory banano)
            : base(data, log)
        {
            Wax = wax;
            Banano = banano;
        }

        protected override Func<Task<Credit>> Get => Data.PullNextCredit;
        protected override async Task Process(Credit credit)
        {
            var account = credit.Account;

            var wax = account.CPU + account.NET;
            var bananoPerDay = wax * Calculations.BananoPerWaxPerDay;
            var days = Convert.ToDouble(credit.Banano / bananoPerDay);

            if (account.PaidThrough.HasValue)
            {
                await Data.ProcessCredit(credit.CreditId, account.PaidThrough.Value.AddDays(days));
            }
            else
            {
                // We're opening the account, so we have to stake the CPU and NET.
                var success = await Wax.Stake(account.WaxAccount, account.CPU, account.NET);
                if (success)
                {
                    await Data.ProcessCredit(credit.CreditId, DateTime.UtcNow.AddDays(days));
                }
            }

            // Send banano to storage.
            await Banano.BuildAccount((uint)account.AccountId).Send(Protocol.Address, credit.Banano);
        }

    }
}
