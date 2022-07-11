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
                return new Entities.App.AppConstants
                {
                    Accounts = new Entities.App.AppAccounts
                    {
                        BananoSweepAddress  = constants.BananoSweepAddress,
                        WaxPrimaryAccount   = constants.WaxPrimaryAccount,
                        WaxTransactAccounts = constants.WaxTransactAccounts
                    },
                    Validations = new Entities.App.Validations
                    {
                        BananoAddressRegex = ServiceConstants.Banano.Protocol.AddressRegex,
                        WaxAccountRegex    = ServiceConstants.Wax.Protocol.AccountRegex
                    },
                    Rentals = new Entities.App.NewRentalInfo
                    {
                        DaysDoubleThreshold = ServiceConstants.Rentals.DaysDoubleThreshold
                    },
                    WelcomePackages = new Entities.App.NewAccountPackageInfo
                    {
                        WaxReceivingAccount = ServiceConstants.Wax.NewUser.Account,
                        WaxSend             = ServiceConstants.Wax.NewUser.OpenWax,
                        WaxMemoRegex        = ServiceConstants.Wax.NewUser.MemoRegex,
                        FreeRentalCpu       = ServiceConstants.Wax.NewUser.FreeCpu,
                        FreeRentalNet       = ServiceConstants.Wax.NewUser.FreeNet,
                        FreeRentalDays      = ServiceConstants.Wax.NewUser.FreeRentalDays
                    }
                };
            }
            return null;
        }

        public Entities.App.AppInsights Map(ServiceEntities.AppInsights insights)
        {
            if (insights != null)
            {
                return new Entities.App.AppInsights
                {
                    MonthlyStats          = insights.MonthlyStats?.Select(Map),
                    LatestRentals         = insights.LatestRentals?.Select(Map),
                    LatestPurchases       = insights.LatestPurchases?.Select(Map),
                    LatestWelcomePackages = insights.LatestWelcomePackages?.Select(Map)
                };
            }
            return null;
        }

        public Entities.App.AppState Map(ServiceEntities.AppState state)
        {
            if (state != null)
            {
                return new Entities.App.AppState
                {
                    Banano = new Entities.App.BananoStateInfo
                    {
                        Balance = state.BananoBalance,
                        Price   = state.BananoPrice
                    },
                    Wax = new Entities.App.WaxStateInfo
                    {
                        Price                       = state.WaxPrice,
                        Staked                      = state.WaxStaked,
                        WorkingAccount              = state.WaxWorkingAccount,
                        AvailableToday              = state.WaxBalanceAvailableToday,
                        AdditionalAvailableTomorrow = state.WaxBalanceAvailableTomorrow
                    },
                    Costs = new Entities.App.CostsInfo
                    {
                        WaxRentPriceInBanano        = state.WaxRentPriceInBanano,
                        WaxBuyPriceInBanano         = state.WaxBuyPriceInBanano,
                        WelcomePackagePriceInBanano = state.BananoWelcomePackagePrice
                    },
                    Limits = new Entities.App.LimitsInfo
                    {
                        BananoMinimumCredit = state.BananoMinimumCredit,
                        WaxMinimumRent      = state.WaxMinimumRent,
                        WaxMaximumRent      = state.WaxMaximumRent,
                        WaxMinimumBuy       = state.WaxMinimumBuy,
                        WaxMaximumBuy       = state.WaxMaximumBuy
                    },
                    WelcomePackages = new Entities.App.WelcomePackagesInfo
                    {
                        WaxAvailable        = state.WelcomePackagesAvailable,
                        FreeRentalAvailable = state.WelcomePackageRentalsAvailable,
                        FreeNftAvailable    = state.WelcomePackageNftsAvailable
                    }
                };
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
                var result = new Entities.Rentals.RentalInfo
                {
                    Payment = new Entities.BananoPaymentInfo
                    {
                        Address = rental.BananoAddress,
                        Amount  = rental.Banano,
                        AppLink = rental.BananoPaymentLink
                    },
                    Target = new Entities.Rentals.WaxTargetInfo
                    {
                        Account = rental.WaxAccount,
                        Cpu     = rental.Cpu,
                        Net     = rental.Net,
                        Days    = rental.Days
                    },
                    Status = Map(rental.Status)
                };
                if (rental.Paid.HasValue)
                {
                    result.Dates = new Entities.Rentals.DatesInfo
                    {
                        Paid    = rental.Paid.Value,
                        Staked  = rental.Staked,
                        Expires = rental.Expires
                    };
                }
                if (rental.StakeTransaction != null || rental.UnstakeTransaction != null)
                {
                    result.Transactions = new Entities.Rentals.TransactionsInfo
                    {
                        WaxStake   = rental.StakeTransaction,
                        WaxUnstake = rental.UnstakeTransaction
                    };
                }
                return result;
            }
            return null;
        }

        public Entities.WelcomePackages.WelcomePackageInfo Map(ServiceEntities.WelcomePackageInfo package)
        {
            if (package != null)
            {
                var result = new Entities.WelcomePackages.WelcomePackageInfo
                {
                    Payment = new Entities.BananoPaymentInfo
                    {
                        Address = package.BananoAddress,
                        Amount  = package.Banano,
                        AppLink = package.BananoPaymentLink
                    },
                    Target = new Entities.WelcomePackages.WaxTargetInfo
                    {
                        Account = package.WaxAccount,
                        Amount  = package.Wax,
                        Memo    = package.Memo
                    },
                    Status = Map(package.Status)
                };
                if (package.FundTransaction != null || package.NftTransaction != null || package.StakeTransaction != null)
                {
                    result.Transactions = new Entities.WelcomePackages.TransactionsInfo
                    {
                        WaxTransfer = package.FundTransaction,
                        NftTransfer = package.NftTransaction,
                        RentalStake = package.StakeTransaction
                    };
                }
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
