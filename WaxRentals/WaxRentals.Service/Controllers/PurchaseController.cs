using Microsoft.AspNetCore.Mvc;
using WaxRentals.Data.Manager;
using WaxRentals.Service.Config;
using WaxRentals.Service.Shared.Entities.Input;

namespace WaxRentals.Service.Controllers
{
    public class PurchaseController : ServiceBase
    {

        private IInsert Insert { get; }
        private IProcess Processor { get; }
        private Mapper Mapper { get; }

        public PurchaseController(
            ILog log,
            IInsert insert,
            IProcess processor,
            Mapper mapper)
            : base(log)
        {
            Insert = insert;
            Processor = processor;
            Mapper = mapper;
        }

        [HttpPost("Create")]
        public async Task<JsonResult> Create([FromBody] NewPurchaseInput input)
        {
            var success = await Insert.OpenPurchase(
                input.Amount,
                input.Transaction,
                input.BananoPaymentAddress,
                input.Banano,
                Mapper.Map(input.Status)
            );
            return success ? Succeed() : Fail("Purchase already exists.");
        }

        [HttpGet("Next")]
        public async Task<JsonResult> Next()
        {
            var purchase = await Processor.PullNextPurchase();
            return Succeed(Mapper.Map(purchase));
        }

        [HttpPost("Process")]
        public async Task<JsonResult> Process([FromBody] ProcessInput input)
        {
            await Processor.ProcessPurchase(input.Id, input.Transaction);
            return Succeed();
        }

    }
}
