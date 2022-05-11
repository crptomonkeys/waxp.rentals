using System;
using System.Threading.Tasks;
using WaxRentals.Data.Entities;

namespace WaxRentals.Data.Access
{
    public interface IProcess
    {

        Task<Credit> PullNextCredit();
        Task<Payment> PullNextPayment();
        Task<Account> PullNextClosingAccount();

        Task ProcessCredit(int creditId, DateTime paidThrough);
        Task ProcessPayment(int paymentId, string bananoTransaction);
        Task ProcessAccountClosing(int accountId);

    }
}
