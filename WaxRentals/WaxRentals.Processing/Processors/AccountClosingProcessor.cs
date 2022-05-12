using System;
using System.Threading.Tasks;
using WaxRentals.Banano.Transact;
using WaxRentals.Data.Entities;
using WaxRentals.Data.Manager;
using static WaxRentals.Processing.Constants;
using WaxAccount = WaxRentals.Waxp.Transact.ITransact;

namespace WaxRentals.Processing.Processors
{
    internal class AccountClosingProcessor : Processor<Account>
    {

        private WaxAccount Wax { get; }
        private IBananoAccountFactory Banano { get; set; }

        public AccountClosingProcessor(IProcess data, ILog log, WaxAccount wax, IBananoAccountFactory banano)
            : base(data, log)
        {
            Wax = wax;
            Banano = banano;
        }

        protected override Func<Task<Account>> Get => Data.PullNextClosingAccount;
        protected override async Task Process(Account account)
        {
            var banano = Banano.BuildAccount((uint)account.AccountId);
            var pending = await banano.HasPendingBlocks() || await Data.HasPendingCredits(account.AccountId);

            // If there are any pending receives for the account, skip this one for now.
            if (pending)
            {
                await Data.ApplyFreeCredit(account.AccountId, Calculations.FreeCredit);
            }
            else
            {
                await Wax.Unstake(account.WaxAccount, account.CPU, account.NET);
                await Data.ProcessAccountClosing(account.AccountId);
            }
        }

    }
}
