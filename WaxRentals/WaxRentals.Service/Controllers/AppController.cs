using Microsoft.AspNetCore.Mvc;
using WaxRentals.Banano.Transact;
using WaxRentals.Data.Manager;
using WaxRentals.Service.Caching;
using WaxRentals.Service.Shared.Entities;
using static WaxRentals.Service.Shared.Config.Constants;

namespace WaxRentals.Service.Controllers
{
    public class AppController : ServiceBase
    {

        private Cache Cache { get; }
        private IBananoAccount Banano { get; }

        public AppController(
            IDataFactory factory,
            
            Cache cache,
            IBananoAccount banano)
            : base(factory)
        {
            Cache = cache;
            Banano = banano;
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
                    BananoAddress                  = Banano.Address,
                    BananoBalance                  = Balance(Cache.BananoInfo.GetBalance()),
                                                   
                    WaxPrice                       = Price(prices.Wax),
                    WaxAccount                     = Wax.PrimaryAccount,
                    WaxStaked                      = Balance(waxInfo.Staked),
                    WaxWorkingAccount              = waxInfo.Today,
                    WaxBalanceAvailableToday       = Balance(waxInfo.Available),
                    WaxBalanceAvailableTomorrow    = Balance(waxInfo.Unstaking),
                                                   
                    WaxRentPriceInBanano           = Price(costs.WaxRentPriceInBanano),
                    WaxBuyPriceInBanano            = Price(costs.WaxBuyPriceInBanano),
                    BananoWelcomePackagePrice      = Price(costs.BananoWelcomePackagePrice),
                                                   
                    BananoMinimumCredit            = Balance(limits.BananoMinimumCredit),
                    WaxMinimumRent                 = Balance(limits.WaxMinimumRent),
                    WaxMaximumRent                 = Balance(limits.WaxMaximumRent),
                    WaxMinimumBuy                  = Balance(limits.WaxMinimumBuy),
                    WaxMaximumBuy                  = Balance(limits.WaxMaximumBuy),

                    WelcomePackagesAvailable       = waxInfo.Available >= Wax.NewUser.OpenWax,
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
            return Succeed(
                Cache.Insights.GetInsights()
            );
        }

        #region " Rounding "

        private decimal Price(decimal value)
        {
            return Math.Round(value, 6);
        }

        private decimal Balance(decimal value)
        {
            return Math.Round(value, 4);
        }

        #endregion

    }
}
