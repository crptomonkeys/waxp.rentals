using System;
using System.Threading.Tasks;

namespace WaxRentals.Data.Manager
{
    public interface ILog
    {

        Task Error(object context, Exception exception);

    }
}
