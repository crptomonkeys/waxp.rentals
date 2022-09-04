using System.Net;
using Microsoft.AspNetCore.Mvc;
using WaxRentals.Service.Shared.Connectors;
using WaxRentals.Service.Shared.Entities;
using WaxRentals.Service.Shared.Entities.Input;

namespace WaxRentals.Manage.Controllers
{
    public class RentalController : ServiceBase
    {

        private IRentalService Rentals { get; }

        public RentalController(IRentalService rentals)
        {
            Rentals = rentals;
        }

        [HttpPost("MarkAsPaid")]
        [ProducesResponseType(typeof(Task<Result<string>>), (int)HttpStatusCode.OK)]
        public async Task<JsonResult> MarkAsPaid([FromForm] int rentalId)
        {
            return Json(await Rentals.ProcessPayment(rentalId));
        }

        [HttpPost("ProvideFree")]
        [ProducesResponseType(typeof(Task<Result<string>>), (int)HttpStatusCode.OK)]
        public async Task<JsonResult> ProvideFree([FromForm] string account, [FromForm] int days, [FromForm] decimal cpu, [FromForm] decimal net)
        {
            return Json(
                await Rentals.Create(
                    new NewRentalInput
                    {
                        Account = account,
                        Days = days,
                        Cpu = cpu,
                        Net = net,
                        Free = true
                    }
                )
            );
        }

        [HttpPost("Extend")]
        [ProducesResponseType(typeof(Task<Result<RentalInfo>>), (int)HttpStatusCode.OK)]
        public async Task<JsonResult> Extend([FromForm] string address, [FromForm] int days)
        {
            return Json(await Rentals.Extend(address, days));
        }

        [HttpPost("Expire")]
        [ProducesResponseType(typeof(Task<Result<RentalInfo>>), (int)HttpStatusCode.OK)]
        public async Task<JsonResult> Expire([FromForm] string address)
        {
            return Json(await Rentals.Expire(address));
        }

    }
}