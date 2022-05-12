using System;
using System.Threading.Tasks;

namespace WaxRentals.Data.Manager
{
    public interface ILog
    {

        Task Error(Exception exception, object context = null);

    }
}
