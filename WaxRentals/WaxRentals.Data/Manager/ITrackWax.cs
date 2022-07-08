using System;
using System.Threading.Tasks;

namespace WaxRentals.Data.Manager
{
    public interface ITrackWax
    {

        Task<DateTime?> GetLastHistoryCheck();
        Task SetLastHistoryCheck(DateTime last);

    }
}
