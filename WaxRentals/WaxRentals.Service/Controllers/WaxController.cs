using WaxRentals.Data.Manager;
using WaxRentals.Waxp.Transact;

namespace WaxRentals.Service.Controllers
{
    public class WaxController : ServiceBase
    {

        private IWaxAccounts Wax { get; }

        public WaxController(
            IDataFactory factory,
            IWaxAccounts wax)
            : base(factory)
        {
            Wax = wax;
        }



    }
}
