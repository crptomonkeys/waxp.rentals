using System;

namespace WaxRentals.Data.Manager
{
    public interface ITrackWax
    {

        DateTime? GetLastHistoryCheck();
        void SetLastHistoryCheck(DateTime last);

    }
}
