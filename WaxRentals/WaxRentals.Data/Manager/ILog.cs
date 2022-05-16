using System;
using System.Threading.Tasks;
using WaxRentals.Data.Entities;

namespace WaxRentals.Data.Manager
{
    public interface ILog
    {

        Task Error(Exception exception, string error = null, object context = null);
        Task Message(Guid requestId, string url, MessageDirection direction, string message);

    }
}
