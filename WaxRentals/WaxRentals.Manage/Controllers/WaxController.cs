using Microsoft.AspNetCore.Mvc;
using WaxRentals.Service.Shared.Connectors;

namespace WaxRentals.Manage.Controllers
{
    public class WaxController : ServiceBase
    {

        private IWaxService Wax { get; set; }

        public WaxController(IWaxService wax)
        {
            Wax = wax;
        }

        [HttpPost("Send")]
        public async Task<JsonResult> Send([FromForm] string recipient, [FromForm] decimal amount, [FromForm] string? memo = null, [FromForm] string? source = null)
        {
            return Json(await Wax.Send(recipient, amount, memo, source));
        }

        [HttpPost("SendAsset")]
        public async Task<JsonResult> SendAsset([FromForm] string recipient, [FromForm] string assetId, [FromForm] string? memo = null, [FromForm] string? source = null)
        {
            return Json(await Wax.SendAsset(recipient, assetId, memo, source));
        }

        [HttpPost("ClaimRefund")]
        public async Task<JsonResult> ClaimRefund([FromForm] string account)
        {
            return Json(await Wax.ClaimRefund(account));
        }

    }
}