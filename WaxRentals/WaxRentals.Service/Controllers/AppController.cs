using Microsoft.AspNetCore.Mvc;
using WaxRentals.Banano.Transact;
using WaxRentals.Data.Manager;
using WaxRentals.Service.Caching;
using WaxRentals.Service.Shared.Entities;
using WaxConstants = WaxRentals.Waxp.Config.Constants;

namespace WaxRentals.Service.Controllers
{
    public class AppController : ServiceBase
    {

        private CostsCache Costs { get; }
        private LimitsCache Limits { get; }

        private IBananoAccount Banano { get; }

        public AppController(
            IDataFactory factory,
            
            CostsCache costs,
            LimitsCache limits,
            
            IBananoAccount banano)
            : base(factory)
        {
            Costs = costs;
            Limits = limits;

            Banano = banano;
        }

        [HttpGet("State")]
        public async Task<JsonResult> State()
        {
            return Succeed(
                new AppState
                {
                    BananoAddress = Banano.Address,

                    WaxAccount = WaxConstants.Protocol.Account
                }
            );
        }

    }
}
