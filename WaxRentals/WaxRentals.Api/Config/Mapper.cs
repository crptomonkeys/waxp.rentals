using ServiceConstants = WaxRentals.Service.Shared.Config.Constants;
using ServiceEntities = WaxRentals.Service.Shared.Entities;

#nullable disable

namespace WaxRentals.Api.Config
{
    public class Mapper
    {

        #region " Inputs "

        public ServiceEntities.Input.NewRentalInput Map(Entities.Rentals.NewRental rental)
        {
            if (rental != null)
            {
                return new ServiceEntities.Input.NewRentalInput
                {
                    Account = rental.Account,
                    Cpu     = rental.Cpu,
                    Net     = rental.Net,
                    Days    = rental.Days,
                    Free    = false
                };
            }
            return null;
        }

        #endregion

        #region " Outputs "

        public Entities.App.AppConstants Map(ServiceEntities.AppConstants constants)
        {
            if (constants != null)
            {
                var result = new Entities.App.AppConstants();
                result.Accounts = new Entities.App.AppAccounts
                {
                    BananoSweepAddress  = constants.BananoSweepAddress,
                    WaxPrimaryAccount   = constants.WaxPrimaryAccount,
                    WaxTransactAccounts = constants.WaxTransactAccounts
                };
                result.Validations = new Entities.App.Validations
                {
                    BananoAddressRegex = ServiceConstants.Banano.Protocol.AddressRegex,
                    WaxAccountRegex    = ServiceConstants.Wax.Protocol.AccountRegex
                };
                result.WelcomePackages = new Entities.App.NewAccountPackageInfo
                {
                    WaxReceivingAccount = ServiceConstants.Wax.NewUser.Account,
                    WaxSend             = ServiceConstants.Wax.NewUser.OpenWax,
                    WaxMemoRegex        = ServiceConstants.Wax.NewUser.MemoRegex,
                    FreeRentalCpu       = ServiceConstants.Wax.NewUser.FreeCpu,
                    FreeRentalNet       = ServiceConstants.Wax.NewUser.FreeNet,
                    FreeRentalDays      = ServiceConstants.Wax.NewUser.FreeRentalDays
                };
                return result;
            }
            return null;
        }

        public Entities.App.AppInsights Map(ServiceEntities.AppInsights insights)
        {
            if (insights != null)
            {
                return new Entities.App.AppInsights
                {
                    MonthlyStats = insights.MonthlyStats?.Select(Map),
                    LatestRentals = insights.LatestRentals?.Select(Map),
                    LatestPurchases = insights.LatestPurchases?.Select(Map),
                    LatestWelcomePackages = insights.LatestWelcomePackages?.Select(Map)
                };
            }
            return null;
        }

        public Entities.App.AppState Map(ServiceEntities.AppState state)
        {
            if (state != null)
            {
                var result = new Entities.App.AppState();

                return result;
            }
            return null;
        }

        public Entities.App.MonthlyStats Map(ServiceEntities.MonthlyStats stats)
        {
            if (stats != null)
            {
                return new Entities.App.MonthlyStats
                {
                    Year                  = stats.Year,
                    Month                 = stats.Month,
                    WaxDaysRented         = stats.WaxDaysRented,
                    WaxDaysFree           = stats.WaxDaysFree,
                    WaxPurchasedForSite   = stats.WaxPurchasedForSite,
                    WelcomePackagesOpened = stats.WelcomePackagesOpened
                };
            }
            return null;
        }

        public Entities.App.PurchaseInfo Map(ServiceEntities.PurchaseInfo purchase)
        {
            if (purchase != null)
            {
                return new Entities.App.PurchaseInfo
                {
                    Wax               = purchase.Wax,
                    WaxTransaction    = purchase.WaxTransaction,
                    Banano            = purchase.Banano,
                    BananoTransaction = purchase.BananoTransaction
                };
            }
            return null;
        }

        public Entities.Rentals.RentalInfo Map(ServiceEntities.RentalInfo rental)
        {
            if (rental != null)
            {
                var result = new Entities.Rentals.RentalInfo();
                result.Payment = new Entities.BananoPaymentInfo
                {
                    Address = rental.BananoAddress,
                    Amount  = rental.Banano,
                    AppLink = rental.BananoPaymentLink
                };
                result.Target = new Entities.Rentals.WaxTargetInfo
                {
                    Account = rental.WaxAccount,
                    Cpu     = rental.Cpu,
                    Net     = rental.Net,
                    Days    = rental.Days
                };
                if (rental.Paid.HasValue)
                {
                    result.Dates = new Entities.Rentals.DatesInfo
                    {
                        Paid    = rental.Paid.Value,
                        Expires = rental.Expires.Value
                    };
                }
                result.Transactions = new Entities.Rentals.TransactionsInfo
                {
                    WaxStake = rental.StakeTransaction,
                    WaxUnstake = rental.UnstakeTransaction
                };
                result.Status = Map(rental.Status);
                return result;
            }
            return null;
        }

        public Entities.WelcomePackages.WelcomePackageInfo Map(ServiceEntities.WelcomePackageInfo package)
        {
            if (package != null)
            {
                var result = new Entities.WelcomePackages.WelcomePackageInfo();
                result.Payment = new Entities.BananoPaymentInfo
                {
                    Address = package.BananoAddress,
                    Amount  = package.Banano,
                    AppLink = package.BananoPaymentLink
                };
                result.Target = new Entities.WelcomePackages.WaxTargetInfo
                {
                    Account = package.WaxAccount,
                    Amount  = package.Wax,
                    Memo    = package.Memo
                };
                result.Transactions = new Entities.WelcomePackages.TransactionsInfo
                {
                    WaxTransfer = package.FundTransaction,
                    NftTransfer = package.NftTransaction,
                    RentalStake = package.StakeTransaction
                };
                result.Status = Map(package.Status);
                return result;
            }
            return null;
        }

        public Entities.Status Map(ServiceEntities.Status status)
        {
            return Enum.Parse<Entities.Status>(status.ToString());
        }

        #endregion

    }
}
