using System.Net;
using Microsoft.AspNetCore.Mvc;
using WaxRentals.Service.Shared.Connectors;
using WaxRentals.Service.Shared.Entities;

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
        [ProducesResponseType(typeof(Task<Result<string>>), (int)HttpStatusCode.OK)]
        public async Task<JsonResult> MarkAsPaid([FromForm] int packageId)
        {
            return Json(await Packages.ProcessPayment(packageId));
        }

    }
}