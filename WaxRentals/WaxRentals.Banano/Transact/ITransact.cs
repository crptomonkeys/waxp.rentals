using System.Threading.Tasks;
using Nano.Net.Numbers;

namespace WaxRentals.Banano.Transact
{
    public interface ITransact
    {

        Task Send(string target, decimal banano);
        Task<BigDecimal> Receive();

    }
}
