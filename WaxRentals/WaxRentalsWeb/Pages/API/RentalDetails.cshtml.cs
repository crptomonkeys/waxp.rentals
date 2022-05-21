using System.Linq;
using System.Text.RegularExpressions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using WaxRentals.Banano.Transact;
using WaxRentals.Data.Manager;
using WaxRentalsWeb.Data.Models;
using static WaxRentals.Waxp.Config.Constants;

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
            if (!string.IsNullOrWhiteSpace(address) && Regex.IsMatch(address, Protocol.BananoAddressRegex))
            {
                var rental = _data.Explore.GetRentalsByBananoAddresses(new string[] { address }).SingleOrDefault();
                if (rental != null)
                {
                    return new JsonResult(new RentalDetailModel(rental, _banano));
                }
            }
            return null;
        }

    }
}
