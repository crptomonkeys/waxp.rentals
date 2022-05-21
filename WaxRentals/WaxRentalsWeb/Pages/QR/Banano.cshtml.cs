using IronBarCode;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Nano.Net;
using WaxRentals.Banano.Transact;
using WaxRentals.Data.Manager;
using static WaxRentals.Waxp.Config.Constants;
using static WaxRentalsWeb.Config.Constants;

namespace WaxRentalsWeb.Pages
{
    public class BananoModel : PageModel
    {

        private readonly IDataFactory _data;
        private readonly IBananoAccountFactory _banano;

        public BananoModel(IDataFactory data, IBananoAccountFactory banano)
        {
            _data = data;
            _banano = banano;
        }

        public IActionResult OnGet(string address)
        {
            if (!string.IsNullOrWhiteSpace(address))
            {
                var rental = _data.Explore.GetRentalByBananoAddress(address);
                if (rental != null)
                {
                    var account = _banano.BuildAccount((uint)rental.RentalId);
                    var link = account.BuildLink(rental.Banano);
                    var qr = QRCodeWriter.CreateQrCodeWithLogo(link, Images.Logo, Images.Size);
                    return File(qr.ToPngBinaryData(), "image/png");
                }
            }
            return null;
        }

    }
}
