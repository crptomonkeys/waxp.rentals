using Microsoft.AspNetCore.Mvc;
using WaxRentals.Service.Shared.Connectors;

namespace WaxRentals.Manage.Controllers
{
    public class BananoController : ServiceBase
    {

        private IBananoService Banano { get; }

        public BananoController(IBananoService banano)
        {
            Banano = banano;
        }

        [HttpPost("Send")]
        public async Task<JsonResult> Send([FromForm] string address, [FromForm] decimal amount)
        {
            return Json(await Banano.Send(address, amount));
        }

    }
}