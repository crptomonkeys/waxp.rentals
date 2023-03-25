using Microsoft.AspNetCore.Mvc;
using WaxRentals.Service.Shared.Connectors;

namespace WaxRentals.Manage.Controllers
{
    public class WelcomePackageController : ServiceBase
    {

        private IWelcomePackageService Packages { get; }

        public WelcomePackageController(IWelcomePackageService packages)
        {
            Packages = packages;
        }

        [HttpPost("MarkAsPaid")]
        public async Task<JsonResult> MarkAsPaid([FromForm] string address)
        {
            var package = await Packages.ByBananoAddress(address);
            if (package.Success)
            {
                return Json(await Packages.ProcessPayment(package.Value.Id));
            }
            return Json(package);
        }

    }
}