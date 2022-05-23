using System.Threading.Tasks;

namespace WaxRentals.Banano.Transact
{
    public interface IBananoAccount
    {

        string Address { get; }
        string BuildLink(decimal amount);

        Task<string> Send(string target, decimal banano);
        Task<decimal> Receive();
        Task<decimal> GetBalance();

        Task<string> GenerateWork();

    }
}
