using Microsoft.AspNetCore.Mvc;
using WaxRentals.Banano.Transact;
using WaxRentals.Data.Manager;
using WaxRentals.Service.Caching;
using WaxRentals.Service.Shared.Entities;
using WaxRentals.Waxp.Transact;
using static WaxRentals.Service.Shared.Config.Constants;

namespace WaxRentals.Service.Controllers
{
    public class AppController : ServiceBase
    {

        private Cache Cache { get; }
        private IBananoAccount Banano { get; }
        private IWaxAccounts WaxAccounts { get; }

        public AppController(
            ILog log,
            
            Cache cache,
            IBananoAccount banano,
            IWaxAccounts waxAccounts)
            : base(log)
        {
            Cache = cache;
            Banano = banano;
            WaxAccounts = waxAccounts;
        }

        [HttpGet("State")]
        public JsonResult State()
        {
            var costs = Cache.Costs.GetCosts();
            var limits = Cache.Limits.GetLimits();
            var prices = Cache.Prices.GetPrices();
            var waxInfo = Cache.WaxInfo.GetBalances();
            return Succeed(
                new AppState
                {
                    BananoPrice                    = Price(prices.Banano),
                    BananoBalance                  = Balance(Cache.BananoInfo.GetBalance()),
                                                   
                    WaxPrice                       = Price(prices.Wax),
                    WaxStaked                      = Balance(waxInfo.Staked),
                    WaxWorkingAccount              = waxInfo.Account,
                    WaxBalanceAvailableToday       = Balance(waxInfo.Available),
                    WaxBalanceAvailableTomorrow    = Balance(waxInfo.Unstaking),
                                                   
                    WaxRentPriceInBanano           = Price(costs.WaxRentPriceInBanano),
                    WaxBuyPriceInBanano            = Price(costs.WaxBuyPriceInBanano),
                    BananoWelcomePackagePrice      = Price(costs.BananoWelcomePackagePrice, 0),
                                                   
                    BananoMinimumCredit            = Balance(limits.BananoMinimumCredit),
                    WaxMinimumRent                 = Balance(limits.WaxMinimumRent),
                    WaxMaximumRent                 = Balance(limits.WaxMaximumRent),
                    WaxMinimumBuy                  = Balance(limits.WaxMinimumBuy),
                    WaxMaximumBuy                  = Balance(limits.WaxMaximumBuy),

                    WelcomePackagesAvailable       = costs.BananoWelcomePackagePrice > 0 &&
                                                     waxInfo.Available >= Wax.NewUser.OpenWax,
                    WelcomePackageRentalsAvailable = waxInfo.Available >= (Wax.NewUser.OpenWax +
                                                                           Wax.NewUser.FreeCpu +
                                                                           Wax.NewUser.FreeNet),
                    WelcomePackageNftsAvailable    = Cache.Nfts.GetNfts().Any()
                }
            );
        }

        [HttpGet("Insights")]
        public JsonResult Insights()
        {
            var insights = Cache.Insights.GetInsights();
            if (insights.LatestPurchases?.Any() == true)
            {
                insights.LatestPurchases = insights.LatestPurchases.ToArray();
                foreach (var purchase in insights.LatestPurchases)
                {
                    purchase.Banano = Balance(purchase.Banano, 4);
                }
            }
            return Succeed(insights);
        }

        [HttpGet("Constants")]
        public JsonResult Constants()
        {
            return Succeed(
                new AppConstants
                {
                    BananoSweepAddress  = Banano.Address,
                    WaxPrimaryAccount   = WaxAccounts.Primary.Account,
                    WaxTransactAccounts = WaxAccounts.Transact.Select(wax => wax.Account)
                }
            );
        }

        #region " Rounding "

        private static decimal Price(decimal value, int decimals = 4)
        {
            return Math.Round(value, decimals);
        }

        private static decimal Balance(decimal value, int decimals = 0)
        {
            return Math.Round(value, decimals);
        }

        #endregion

    }
}
