using System.Text.RegularExpressions;
using Microsoft.AspNetCore.Mvc;
using WaxRentals.Banano.Transact;
using WaxRentals.Data.Manager;
using WaxRentals.Service.Caching;
using WaxRentals.Service.Config;
using WaxRentals.Service.Shared.Entities;
using WaxRentals.Service.Shared.Entities.Input;
using static WaxRentals.Service.Shared.Config.Constants.Wax;

namespace WaxRentals.Service.Controllers
{
    public class WelcomePackageController : ServiceBase
    {

        private CostsCache Costs { get; }
        private IBananoAccountFactory Banano { get; }
        private Mapper Mapper { get; }

        public WelcomePackageController(IDataFactory factory, CostsCache costs, IBananoAccountFactory banano, Mapper mapper)
            : base(factory)
        {
            Costs = costs;
            Banano = banano;
            Mapper = mapper;
        }

        #region " Create "

        [HttpPost("Create")]
        public async Task<JsonResult> Create([FromBody] string memo)
        {
            try
            {
                // Filter invalid memos.
                if (string.IsNullOrWhiteSpace(memo) || !Regex.IsMatch(memo, NewUser.MemoRegex))
                {
                    return Fail("Please check that the memo provided is correct.");
                }

                var cost = Costs.GetCosts().BananoWelcomePackagePrice;
                if (cost == 0)
                {
                    return Fail("Something went wrong; please try again in a few minutes.");
                }

                var id = await Factory.Insert.OpenWelcomePackage(NewUser.Account, memo, NewUser.OpenWax, cost);
                var account = Banano.BuildWelcomeAccount(id);
                return Succeed(
                    new NewWelcomePackage(
                        account.Address,
                        account.BuildLink(cost),
                        NewUser.Account,
                        memo)
                );
            }
            catch (Exception ex)
            {
                try
                {
                    await Factory.Log.Error(ex);
                    return Fail(ex.Message);
                }
                catch
                {
                    return Fail("Unknown error.");
                }
            }
        }

        #endregion

        #region " Read "

        [HttpPost("ByBananoAddresses")]
        public async Task<JsonResult> ByBananoAddresses([FromBody] IEnumerable<string> addresses)
        {
            var packages = await Factory.Explore.GetWelcomePackagesByBananoAddresses(addresses);
            return Succeed(packages.Select(Mapper.Map));
        }

        [HttpGet("New")]
        public async Task<JsonResult> New()
        {
            var packages = await Factory.Process.PullNewWelcomePackages();
            return Succeed(packages.Select(Mapper.Map));
        }

        [HttpGet("Paid")]
        public async Task<JsonResult> Paid()
        {
            var packages = await Factory.Process.PullPaidWelcomePackagesToFund();
            return Succeed(packages.Select(Mapper.Map));
        }

        [HttpGet("MissingNfts")]
        public async Task<JsonResult> MissingNfts()
        {
            var packages = await Factory.Process.PullFundedWelcomePackagesMissingNft();
            return Succeed(packages.Select(Mapper.Map));
        }

        [HttpGet("MissingRentals")]
        public async Task<JsonResult> MissingRentals()
        {
            var packages = await Factory.Process.PullFundedWelcomePackagesMissingRental();
            return Succeed(packages.Select(Mapper.Map));
        }

        [HttpGet("Sweepable")]
        public async Task<JsonResult> Sweepable()
        {
            var packages = await Factory.Process.PullSweepableWelcomePackages();
            return Succeed(packages.Select(Mapper.Map));
        }

        #endregion

        #region " Update "

        [HttpPost("ProcessPayment")]
        public async Task<JsonResult> ProcessPayment([FromBody] ProcessInput input)
        {
            await Factory.Process.ProcessWelcomePackagePayment(input.Id);
            return Succeed();
        }

        [HttpPost("ProcessFunding")]
        public async Task<JsonResult> ProcessFunding([FromBody] ProcessInput input)
        {
            await Factory.Process.ProcessWelcomePackageFunding(input.Id, input.Transaction);
            return Succeed();
        }

        [HttpPost("ProcessNft")]
        public async Task<JsonResult> ProcessNft([FromBody] ProcessInput input)
        {
            await Factory.Process.ProcessWelcomePackageNft(input.Id, input.Transaction);
            return Succeed();
        }

        [HttpPost("ProcessRental")]
        public async Task<JsonResult> ProcessRental([FromBody] ProcessWelcomePackageInput input)
        {
            await Factory.Process.ProcessWelcomePackageRental(input.Id, input.RentalId);
            return Succeed();
        }

        [HttpPost("ProcessSweep")]
        public async Task<JsonResult> ProcessSweep([FromBody] ProcessInput input)
        {
            await Factory.Process.ProcessWelcomePackageSweep(input.Id, input.Transaction);
            return Succeed();
        }

        #endregion

    }
}
