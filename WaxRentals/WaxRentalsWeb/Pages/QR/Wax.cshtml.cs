using IronBarCode;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using static WaxRentals.Waxp.Config.Constants;
using static WaxRentalsWeb.Config.Constants;

namespace WaxRentalsWeb.Pages
{
    public class WaxModel : PageModel
    {

        public IActionResult OnGet()
        {
            var qr = QRCodeWriter.CreateQrCodeWithLogo(Protocol.Account, Images.Logo, Images.Size);
            return File(qr.ToPngBinaryData(), "image/png");
        }

    }
}
