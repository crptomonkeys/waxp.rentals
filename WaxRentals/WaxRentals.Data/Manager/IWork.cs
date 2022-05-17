using System.Threading.Tasks;

namespace WaxRentals.Data.Manager
{
    public interface IWork
    {

        Task<int> PullNextAddress();
        Task SaveWork(int addressId, string work);

    }
}
