using Microsoft.AspNetCore.Mvc;
using WaxRentals.Data.Manager;
using WaxRentals.Service.Config;
using WaxRentals.Service.Shared.Entities.Input;

namespace WaxRentals.Service.Controllers
{
    public class PurchaseController : ServiceBase
    {

        private Mapper Mapper { get; }

        public PurchaseController(
            IDataFactory factory,
            Mapper mapper)
            : base(factory)
        {
            Mapper = mapper;
        }

        [HttpGet("Next")]
        public async Task<JsonResult> Next()
        {
            var purchase = await Factory.Process.PullNextPurchase();
            return Succeed(Mapper.Map(purchase));
        }

        [HttpPost("Process")]
        public async Task<JsonResult> Process([FromBody] ProcessInput input)
        {
            await Factory.Process.ProcessPurchase(input.Id, input.Transaction);
            return Succeed();
        }

    }
}
