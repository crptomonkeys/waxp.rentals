using System.Collections.Generic;
using System.Threading.Tasks;
using WaxRentals.Data.Entities;

namespace WaxRentals.Data.Manager
{
    public interface IManage
    {

        Task<Rental> ExtendRental(string address, int days);
        Task<Rental> ExpireRental(string address);

    }
}
