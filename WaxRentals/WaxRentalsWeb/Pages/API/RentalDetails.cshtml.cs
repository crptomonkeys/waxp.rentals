using System.Linq;
using System.Text.RegularExpressions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using WaxRentals.Banano.Transact;
using WaxRentals.Data.Entities;
using WaxRentals.Data.Manager;
using WaxRentalsWeb.Data.Models;
using static WaxRentals.Service.Shared.Config.Constants.Banano;

namespace WaxRentalsWeb.Pages
{
    public class RentalDetailsModel : PageModel
    {

        private readonly IDataFactory _data;
        private readonly IBananoAccountFactory _banano;

        public RentalDetailsModel(IDataFactory data, IBananoAccountFactory banano)
        {
            _data = data;
            _banano = banano;
        }

        public JsonResult OnGet(string address)
        {
            // Filter invalid accounts.
            if (!string.IsNullOrWhiteSpace(address) && Regex.IsMatch(address, Protocol.AddressRegex))
            {
                var rental = _data.Explore.GetRentalsByBananoAddresses(new string[] { address }).SingleOrDefault();
                if (rental?.Status == Status.New || rental?.Status == Status.Pending)
                {
                    return new JsonResult(new RentalDetailModel(rental, _banano));
                }
            }
            return null;
        }

    }
}
