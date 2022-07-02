using WaxRentals.Banano.Transact;
using WaxRentals.Service.Shared.Entities;

namespace WaxRentals.Service.Config
{
    public class Mapper
    {

        private IBananoAccountFactory Banano { get; }

        public Mapper(IBananoAccountFactory banano)
        {
            Banano = banano;
        }

        public MonthlyStats Map(Data.Entities.MonthlyStats stats)
        {
            return new MonthlyStats
            {
                Year                  = stats.Year,
                Month                 = stats.Month,
                WaxDaysRented         = stats.WaxDaysRented,
                WaxDaysFree           = stats.WaxDaysFree,
                WaxPurchasedForSite   = stats.WaxPurchasedForSite,
                WelcomePackagesOpened = stats.WelcomePackagesOpened
            };
        }

        public RentalInfo Map(Data.Entities.Rental rental)
        {
            var account = Banano.BuildAccount(rental.RentalId);
            return new RentalInfo
            {
                WaxAccount         = rental.TargetWaxAccount,
                Cpu                = Convert.ToInt32(rental.CPU),
                Net                = Convert.ToInt32(rental.NET),
                Days               = rental.RentalDays,
                Banano             = rental.Banano,
                BananoAddress      = account.Address,
                BananoPaymentLink  = account.BuildLink(rental.Banano),
                Paid               = rental.Paid,
                Expires            = rental.PaidThrough,
                StakeTransaction   = rental.StakeWaxTransaction,
                UnstakeTransaction = rental.UnstakeWaxTransaction,
                Status             = Map(rental.Status)
            };
        }

        public PurchaseInfo Map(Data.Entities.Purchase purchase)
        {
            return new PurchaseInfo
            {
                Wax               = purchase.Wax,
                WaxTransaction    = purchase.WaxTransaction,
                Banano            = purchase.Banano,
                BananoTransaction = purchase.BananoTransaction
            };
        }

        public WelcomePackageInfo Map(Data.Entities.WelcomePackage package)
        {
            var account = Banano.BuildWelcomeAccount(package.PackageId);
            return new WelcomePackageInfo
            {
                Banano            = package.Banano,
                BananoAddress     = account.Address,
                BananoPaymentLink = account.BuildLink(package.Banano),
                Wax               = package.Wax,
                FundTransaction   = package.FundTransaction,
                NftTransaction    = package.NftTransaction,
                StakeTransaction  = package.Rental?.StakeWaxTransaction,
                Status            = Map(package.Status)
            };
        }

        public Status Map(Data.Entities.Status status)
        {
            return Enum.Parse<Status>(status.ToString());
        }

    }
}
