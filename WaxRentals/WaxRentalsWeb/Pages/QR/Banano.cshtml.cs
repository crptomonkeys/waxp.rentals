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
            return await Process(Proxy.Endpoints.RentalByBananoAddress, address);
        }

        public async Task<IActionResult> OnGetWelcome(string address)
        {
            return await Process(Proxy.Endpoints.WelcomePackageByBananoAddress, address);
        }

        private async Task<IActionResult> Process(string endpoint, string address)
        {
            if (!string.IsNullOrWhiteSpace(address))
            {
                var result = await Proxy.Get<RentalInfo>(endpoint, address);
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
