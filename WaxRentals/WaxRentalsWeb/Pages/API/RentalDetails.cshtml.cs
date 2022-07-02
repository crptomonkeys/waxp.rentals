using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using WaxRentals.Service.Shared.Connectors;
using WaxRentals.Service.Shared.Entities;
using WaxRentalsWeb.Data.Models;
using static WaxRentals.Service.Shared.Config.Constants.Banano;

namespace WaxRentalsWeb.Pages
{
    public class RentalDetailsModel : PageModel
    {

        private IRentalService Rentals { get; }

        public RentalDetailsModel(IRentalService rentals)
        {
            Rentals = rentals;
        }

        public async Task<JsonResult> OnGet(string address)
        {
            // Filter invalid accounts.
            if (!string.IsNullOrWhiteSpace(address) && Regex.IsMatch(address, Protocol.AddressRegex))
            {
                var result = await Rentals.ByBananoAddress(address);
                if (result.Success && result.Value != null)
                {
                    var rental = result.Value;
                    if (rental.Status == Status.New || rental.Status == Status.Pending)
                    {
                        return new JsonResult(new RentalDetailModel(result.Value));
                    }
                }
            }
            return null;
        }

    }
}
