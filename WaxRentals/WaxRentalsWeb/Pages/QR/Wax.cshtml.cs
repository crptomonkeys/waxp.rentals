using Microsoft.AspNetCore.Mvc;
using WaxRentalsWeb.Pages.QR;
using static WaxRentals.Waxp.Config.Constants;

namespace WaxRentalsWeb.Pages
{
    public class WaxModel : QRPageModel
    {

        public IActionResult OnGet()
        {
            return GenerateQRCode(Protocol.Account);
        }

    }
}
