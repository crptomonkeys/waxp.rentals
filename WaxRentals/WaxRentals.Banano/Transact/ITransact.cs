using System.Threading.Tasks;
using Nano.Net.Numbers;

namespace WaxRentals.Banano.Transact
{
    public interface ITransact
    {

        public string Address { get; }

        Task<bool> HasPendingBlocks();
        Task<string> Send(string target, BigDecimal banano);
        Task<BigDecimal> Receive();

    }
}
