using System.Linq;
using Microsoft.AspNetCore.Mvc;
using WaxRentals.Banano.Transact;
using WaxRentals.Data.Entities;
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

        public IActionResult OnGetRental(string address)
        {
            if (!string.IsNullOrWhiteSpace(address))
            {
                var rental = _data.Explore.GetRentalsByBananoAddresses(new string[] { address }).SingleOrDefault();
                if (rental?.Status == Status.New || rental?.Status == Status.Pending)
                {
                    var account = _banano.BuildAccount(rental.RentalId);
                    var link = account.BuildLink(rental.Banano);
                    return GenerateQRCode(link);
                }
            }
            return null;
        }

        public IActionResult OnGetWelcome(string address)
        {
            if (!string.IsNullOrWhiteSpace(address))
            {
                var package = _data.Explore.GetWelcomePackagesByBananoAddresses(new string[] { address }).SingleOrDefault();
                if (package?.Status == Status.New)
                {
                    var account = _banano.BuildWelcomeAccount(package.PackageId);
                    var link = account.BuildLink(package.Banano);
                    return GenerateQRCode(link);
                }
            }
            return null;
        }

    }
}
