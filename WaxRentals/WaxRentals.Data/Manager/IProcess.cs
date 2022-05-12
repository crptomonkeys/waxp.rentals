using System;
using System.Threading.Tasks;
using WaxRentals.Data.Entities;

namespace WaxRentals.Data.Manager
{
    public interface IProcess
    {

        Task<Credit> PullNextCredit();
        Task<Payment> PullNextPayment();
        Task<Account> PullNextClosingAccount();

        Task<bool> HasPendingCredits(int accountId);

        Task ApplyFreeCredit(int accountId, TimeSpan free);

        Task ProcessCredit(int creditId, DateTime paidThrough);
        Task ProcessPayment(int paymentId, string bananoTransaction);
        Task ProcessAccountClosing(int accountId);

    }
}
