using Microsoft.AspNetCore.Mvc;
using WaxRentals.Service.Shared.Connectors;
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
        public async Task<JsonResult> MarkAsPaid([FromForm] string address)
        {
            var rental = await Rentals.ByBananoAddress(address);
            if (rental.Success)
            {
                return Json(await Rentals.ProcessPayment(rental.Value.Id));
            }
            return Json(rental);
        }

        [HttpPost("ProvideFree")]
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
        public async Task<JsonResult> Extend([FromForm] string address, [FromForm] int days)
        {
            return Json(await Rentals.Extend(address, days));
        }

        [HttpPost("Expire")]
        public async Task<JsonResult> Expire([FromForm] string address)
        {
            return Json(await Rentals.Expire(address));
        }

    }
}