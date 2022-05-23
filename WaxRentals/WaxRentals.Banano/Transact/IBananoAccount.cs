using System.Threading.Tasks;
using Nano.Net.Numbers;

namespace WaxRentals.Banano.Transact
{
    public interface IBananoAccount
    {

        string Address { get; }
        string BuildLink(decimal amount);

        Task<string> Send(string target, BigDecimal banano);
        Task<BigDecimal> Receive();
        Task<BigDecimal> GetBalance();

        Task<string> GenerateWork();

    }
}
