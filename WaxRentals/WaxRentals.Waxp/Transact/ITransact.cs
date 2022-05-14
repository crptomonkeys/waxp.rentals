using System.Threading.Tasks;

namespace WaxRentals.Waxp.Transact
{
    public interface ITransact
    {

        Task<bool> Stake(string account, decimal cpu, decimal net);
        Task<bool> Unstake(string account, decimal cpu, decimal net);
        Task<bool> CompleteRefund();

    }
}
