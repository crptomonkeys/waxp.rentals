using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using WaxRentals.Service.Shared.Connectors;
using WaxRentals.Service.Shared.Entities;
using WaxRentalsWeb.Pages.QR;

namespace WaxRentalsWeb.Pages
{
    public class BananoModel : QRPageModel
    {

        private IRentalService Rentals { get; }
        private IWelcomePackageService WelcomePackages { get; }

        public BananoModel(IRentalService rentals, IWelcomePackageService welcomePackages)
        {
            Rentals = rentals;
            WelcomePackages = welcomePackages;
        }

        public async Task<IActionResult> OnGetRental(string address)
        {
            // Filter invalid accounts.
            if (!string.IsNullOrWhiteSpace(address))
            {
                var result = await Rentals.ByBananoAddress(address);
                if (result.Success && result.Value != null)
                {
                    var rental = result.Value;
                    if (rental.Status == Status.New || rental.Status == Status.Pending)
                    {
                        return GenerateQRCode(rental.BananoPaymentLink);
                    }
                }
            }
            return null;
        }

        public async Task<IActionResult> OnGetWelcome(string address)
        {
            // Filter invalid accounts.
            if (!string.IsNullOrWhiteSpace(address))
            {
                var result = await WelcomePackages.ByBananoAddress(address);
                if (result.Success && result.Value != null)
                {
                    var package = result.Value;
                    if (package.Status == Status.New)
                    {
                        return GenerateQRCode(package.BananoPaymentLink);
                    }
                }
            }
            return null;
        }

    }
}
