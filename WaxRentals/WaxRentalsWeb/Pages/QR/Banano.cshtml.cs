using System.Linq;
using Microsoft.AspNetCore.Mvc;
using WaxRentals.Banano.Transact;
using WaxRentals.Data.Manager;
using WaxRentalsWeb.Pages.QR;

namespace WaxRentalsWeb.Pages
{
    public class BananoModel : QRPageModel
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
                var rental = _data.Explore.GetRentalsByBananoAddresses(new string[] { address }).SingleOrDefault();
                if (rental != null)
                {
                    var account = _banano.BuildAccount((uint)rental.RentalId);
                    var link = account.BuildLink(rental.Banano);
                    return GenerateQRCode(link);
                }
            }
            return null;
        }

    }
}
