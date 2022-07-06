using Microsoft.AspNetCore.Mvc;
using WaxRentals.Banano.Transact;
using WaxRentals.Data.Manager;
using WaxRentals.Service.Caching;
using WaxRentals.Service.Config;
using WaxRentals.Service.Shared.Entities;
using WaxRentals.Service.Shared.Entities.Input;
using static WaxRentals.Service.Config.Constants;
using Status = WaxRentals.Data.Entities.Status;

namespace WaxRentals.Service.Controllers
{
    public class RentalController : ServiceBase
    {

        private IInsert Insert { get; }
        private IProcess Process { get; }
        private IExplore Explore { get; }

        private CostsCache Costs { get; }
        private LimitsCache Limits { get; }

        private IBananoAccountFactory Banano { get; }
        private Mapper Mapper { get; }

        public RentalController(
            ILog log,
            IInsert insert,
            IProcess process,
            IExplore explore,
            
            CostsCache costs,
            LimitsCache limits,
            
            IBananoAccountFactory banano,
            Mapper mapper)
            : base(log)
        {
            Insert = insert;
            Process = process;
            Explore = explore;

            Costs = costs;
            Limits = limits;

            Banano = banano;
            Mapper = mapper;
        }

        #region " Create "

        [HttpPost("Create")]
        public async Task<JsonResult> Create([FromBody] NewRentalInput input)
        {
            try
            {
                // Validate.

                var limits = Limits.GetLimits();

                if (input.Cpu + input.Net < limits.WaxMinimumRent)
                {
                    return Fail($"Must rent at least {limits.WaxMinimumRent} WAX.");
                }
                else if (input.Cpu + input.Net > limits.WaxMaximumRent)
                {
                    return Fail($"Cannot rent more than {limits.WaxMaximumRent} WAX in one transaction right now.");
                }
                else if (input.Days < 1)
                {
                    return Fail("Must rent for at least one day.");
                }

                // Open.

                int id;
                if (input.Free)
                {
                    id = await Insert.OpenRental(input.Account, input.Days, input.Cpu, input.Net, 0, Status.Pending);
                }
                else
                {
                    var cost = (input.Cpu + input.Net) * input.Days * Costs.GetCosts().WaxRentPriceInBanano;
                    cost = decimal.Round(cost, 4);

                    id = await Insert.OpenRental(input.Account, RentalDays(input.Days), input.Cpu, input.Net, cost);
                }

                var account = Banano.BuildAccount(id);
                return Succeed(new NewRental(id, account.Address));
            }
            catch (Exception ex)
            {
                try
                {
                    await Log.Error(ex);
                    return Fail(ex.Message);
                }
                catch
                {
                    return Fail("Unknown error.");
                }
            }
        }

        private int RentalDays(int days)
        {
            return (days >= Calculations.DaysDoubleThreshold) ? (days * 2) : days;
        }

        #endregion

        #region " Read "

        [HttpGet("ByWaxAccount/{account}")]
        public async Task<JsonResult> ByWaxAccount(string account)
        {
            var rentals = await Explore.GetRentalsByWaxAccount(account);
            return Succeed(rentals.Select(Mapper.Map));
        }

        [HttpPost("ByBananoAddresses")]
        public async Task<JsonResult> ByBananoAddresses([FromBody] IEnumerable<string> addresses)
        {
            var rentals = await Explore.GetRentalsByBananoAddresses(addresses);
            return Succeed(rentals.Select(Mapper.Map));
        }

        [HttpGet("New")]
        public async Task<JsonResult> New()
        {
            var rentals = await Process.PullNewRentals();
            return Succeed(rentals.Select(Mapper.Map));
        }

        [HttpGet("Paid")]
        public async Task<JsonResult> Paid()
        {
            var rentals = await Process.PullPaidRentalsToStake();
            return Succeed(rentals.Select(Mapper.Map));
        }

        [HttpGet("Sweepable")]
        public async Task<JsonResult> Sweepable()
        {
            var rentals = await Process.PullSweepableRentals();
            return Succeed(rentals.Select(Mapper.Map));
        }

        [HttpGet("NextClosing")]
        public async Task<JsonResult> NextClosing()
        {
            var rental = await Process.PullNextClosingRental();
            return Succeed(Mapper.Map(rental));
        }

        #endregion

        #region " Update "

        [HttpPost("ProcessPayment")]
        public async Task<JsonResult> ProcessPayment([FromBody] ProcessInput input)
        {
            await Process.ProcessRentalPayment(input.Id);
            return Succeed();
        }

        [HttpPost("ProcessStake")]
        public async Task<JsonResult> ProcessStake([FromBody] ProcessRentalInput input)
        {
            await Process.ProcessRentalStaking(input.Id, input.Source, input.Transaction);
            return Succeed();
        }

        [HttpPost("ProcessSweep")]
        public async Task<JsonResult> ProcessSweep([FromBody] ProcessInput input)
        {
            await Process.ProcessRentalSweep(input.Id, input.Transaction);
            return Succeed();
        }

        [HttpPost("ProcessClosing")]
        public async Task<JsonResult> ProcessClosing([FromBody] ProcessInput input)
        {
            await Process.ProcessRentalClosing(input.Id, input.Transaction);
            return Succeed();
        }

        #endregion

    }
}
