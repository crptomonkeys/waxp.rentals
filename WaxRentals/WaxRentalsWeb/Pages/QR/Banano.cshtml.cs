using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using WaxRentals.Api.Entities;
using WaxRentals.Api.Entities.Rentals;
using WaxRentalsWeb.Net;
using WaxRentalsWeb.Pages.QR;

namespace WaxRentalsWeb.Pages
{
    public class BananoModel : QRPageModel
    {

        private ApiProxy Proxy { get; }

        public BananoModel(ApiProxy proxy)
        {
            Proxy = proxy;
        }

        public async Task<IActionResult> OnGetRental(string address)
        {
            if (!string.IsNullOrWhiteSpace(address))
            {
                var result = await Proxy.Get<RentalInfo>(Proxy.Endpoints.RentalByBananoAddress, address);
                if (result.Success && result.Value != null)
                {
                    var rental = result.Value;
                    if (rental.Status == Status.New || rental.Status == Status.Pending)
                    {
                        return GenerateQRCode(rental.Payment.AppLink);
                    }
                }
            }
            return null;
        }

        public async Task<IActionResult> OnGetWelcome(string address)
        {
            if (!string.IsNullOrWhiteSpace(address))
            {
                var result = await Proxy.Get<RentalInfo>(Proxy.Endpoints.WelcomePackageByBananoAddress, address);
                if (result.Success && result.Value != null)
                {
                    var package = result.Value;
                    if (package.Status == Status.New)
                    {
                        return GenerateQRCode(package.Payment.AppLink);
                    }
                }
            }
            return null;
        }

    }
}
