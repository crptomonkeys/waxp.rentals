using System.Threading.Tasks;
using Nano.Net.Numbers;

namespace WaxRentals.Banano.Transact
{
    public interface ITransact
    {

        public string Address { get; }

        Task<string> Send(string target, BigDecimal banano);
        Task<BigDecimal> Receive(bool verifyOnly);
        Task<BigDecimal> GetBalance();

    }
}
