using Microsoft.AspNetCore.Mvc;
using WaxRentals.Banano.Transact;
using WaxRentals.Data.Manager;
using WaxRentals.Service.Shared.Entities.Input;
using static WaxRentals.Service.Shared.Config.Constants.Banano;

namespace WaxRentals.Service.Controllers
{
    public class BananoController : ServiceBase
    {

        private IBananoAccountFactory Banano { get; }
        private IBananoAccount Storage { get; }

        public BananoController(
            IDataFactory factory,
            IBananoAccountFactory banano,
            IBananoAccount storage)
            : base(factory)
        {
            Banano = banano;
            Storage = storage;
        }

        [HttpPost("SweepRentalAccount")]
        public async Task<JsonResult> SweepRentalAccount([FromBody] int id)
        {
            var account = Banano.BuildAccount(id);
            var amount = await account.GetBalance();
            if (amount > 0)
            {
                return Succeed(await account.Send(SweepAddress, amount));
            }
            return Fail("No balance.");
        }

        [HttpPost("SweepWelcomeAccount")]
        public async Task<JsonResult> SweepWelcomeAccount([FromBody] int id)
        {
            var account = Banano.BuildWelcomeAccount(id);
            var amount = await account.GetBalance();
            if (amount > 0)
            {
                return Succeed(await account.Send(SweepAddress, amount));
            }
            return Fail("No balance.");
        }

        [HttpPost("Send")]
        public async Task<JsonResult> Send([FromBody] SendInput input)
        {
            var available = await Storage.GetBalance();
            if (available >= input.Amount)
            {
                return Succeed(await Storage.Send(input.Recipient, input.Amount));
            }
            return Fail($"Requested {input.Amount} banano but only have {available} banano available.");
        }

    }
}
