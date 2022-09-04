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
        public async Task<JsonResult> MarkAsPaid([FromForm] int packageId)
        {
            return Json(await Packages.ProcessPayment(packageId));
        }

    }
}