using System.Threading.Tasks;
using WaxRentals.Data.Entities;

namespace WaxRentals.Data.Manager
{
    public interface IInsert
    {

        Task<Address> OpenAccount(string waxAccount, decimal cpu, decimal net);
        Task ApplyCredit(int accountId, decimal banano, string bananoTransaction);
        Task ApplyPayment(string waxAccount, decimal wax, string waxTransaction, string bananoAddress, decimal banano, Status status);

    }
}
