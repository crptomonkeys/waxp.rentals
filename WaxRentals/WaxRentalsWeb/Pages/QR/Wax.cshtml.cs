using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using WaxRentals.Api.Entities.App;
using WaxRentalsWeb.Net;
using WaxRentalsWeb.Pages.QR;

namespace WaxRentalsWeb.Pages
{
    public class WaxModel : QRPageModel
    {

        private ApiProxy Proxy { get; }

        public WaxModel(ApiProxy proxy)
        {
            Proxy = proxy;
        }

        public async Task<IActionResult> OnGet()
        {
            var constants = await Proxy.Get<AppConstants>(Proxy.Endpoints.AppConstants);
            return constants.Success ? GenerateQRCode(constants.Value.Accounts.WaxPrimaryAccount) : null;
        }

    }
}
