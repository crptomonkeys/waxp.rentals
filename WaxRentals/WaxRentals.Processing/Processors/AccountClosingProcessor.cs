using System.Threading.Tasks;
using WaxRentals.Banano.Transact;
using WaxRentals.Data.Entities;
using WaxRentals.Data.Manager;
using static WaxRentals.Processing.Constants;
using WaxAccount = WaxRentals.Waxp.Transact.ITransact;

namespace WaxRentals.Processing.Processors
{
    internal class AccountClosingProcessor : Processor
    {

        private readonly IProcess _data;
        private readonly WaxAccount _wax;
        private readonly IBananoAccountFactory _banano;

        public AccountClosingProcessor(IProcess data, WaxAccount wax, IBananoAccountFactory banano)
        {
            _data = data;
            _wax = wax;
            _banano = banano;
        }

        protected async override Task Run()
        {
            // Process accounts one at a time.
            // Revisit if this ends up being too slow.
            var account = await _data.PullNextClosingAccount();
            while (account != null)
            {
                await Process(account);
                account = await _data.PullNextClosingAccount();
            }
        }

        private async Task Process(Account account)
        {
            var banano = _banano.BuildAccount((uint)account.AccountId);
            var pending = await banano.HasPendingBlocks() || await _data.HasPendingCredits(account.AccountId);

            // If there are any pending receives for the account, skip this one for now.
            if (pending)
            {
                await _data.ApplyFreeCredit(account.AccountId, Calculations.FreeCredit);
            }
            else
            {
                await _wax.Unstake(account.WaxAccount, account.CPU, account.NET);
                await _data.ProcessAccountClosing(account.AccountId);
            }
        }

    }
}
