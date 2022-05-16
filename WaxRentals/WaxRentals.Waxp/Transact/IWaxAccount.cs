using System.Threading.Tasks;

namespace WaxRentals.Waxp.Transact
{
    public interface IWaxAccount
    {

        public string Account { get; }

        Task<AccountBalances> GetBalances();
        Task<(bool, string)> Stake(string account, decimal cpu, decimal net);
        Task<(bool, string)> Unstake(string account, decimal cpu, decimal net);
        Task<(bool, string)> ClaimRefund();
        Task<(bool, string)> Send(string account, decimal wax);

    }
}
