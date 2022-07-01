using System.Text.RegularExpressions;
using Microsoft.AspNetCore.Mvc;
using WaxRentals.Banano.Transact;
using WaxRentals.Data.Manager;
using WaxRentals.Service.Caching;
using WaxRentals.Service.Shared.Entities;
using static WaxRentals.Service.Shared.Config.Constants.Wax;

namespace WaxRentals.Service.Controllers
{
    public class WelcomePackageController : ServiceBase
    {

        private CostsCache Costs { get; }
        private IBananoAccountFactory Banano { get; }

        public WelcomePackageController(IDataFactory factory, CostsCache costs, IBananoAccountFactory banano)
            : base(factory)
        {
            Costs = costs;
            Banano = banano;
        }

        [HttpPost("New")]
        public async Task<JsonResult> New([FromBody] string memo)
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

    }
}
