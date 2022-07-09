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

        public MonthlyStats? Map(Data.Entities.MonthlyStats stats)
        {
            if (stats != null)
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
            return null;
        }

        public RentalInfo? Map(Data.Entities.Rental rental)
        {
            if (rental != null)
            {
                var account = Banano.BuildAccount(rental.RentalId);
                return new RentalInfo
                {
                    Id                 = rental.RentalId,
                    WaxAccount         = rental.TargetWaxAccount,
                    SourceAccount      = rental.SourceWaxAccount,
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
            return null;
        }

        public PurchaseInfo? Map(Data.Entities.Purchase purchase)
        {
            if (purchase != null)
            {
                return new PurchaseInfo
                {
                    Id                = purchase.PurchaseId,
                    Wax               = purchase.Wax,
                    WaxTransaction    = purchase.WaxTransaction,
                    Banano            = purchase.Banano,
                    BananoAddress     = purchase.PaymentBananoAddress,
                    BananoTransaction = purchase.BananoTransaction
                };
            }
            return null;
        }

        public WelcomePackageInfo? Map(Data.Entities.WelcomePackage package)
        {
            if (package != null)
            {
                var account = Banano.BuildWelcomeAccount(package.PackageId);
                return new WelcomePackageInfo
                {
                    Id                = package.PackageId,
                    Banano            = package.Banano,
                    BananoAddress     = account.Address,
                    BananoPaymentLink = account.BuildLink(package.Banano),
                    WaxAccount        = package.TargetWaxAccount,
                    Wax               = package.Wax,
                    Memo              = package.Memo,
                    FundTransaction   = package.FundTransaction,
                    NftTransaction    = package.NftTransaction,
                    StakeTransaction  = package.Rental?.StakeWaxTransaction,
                    Status            = Map(package.Status)
                };
            }
            return null;
        }

        public WaxTransferInfo? Map(Waxp.History.TransferInfo transfer)
        {
            if (transfer != null)
            {
                return new WaxTransferInfo
                {
                    Source                = transfer.Source,
                    Transaction           = transfer.Transaction,
                    Amount                = transfer.Amount,
                    BananoPaymentAddress  = transfer.BananoPaymentAddress,
                    SkipPayment           = transfer.SkipPayment
                };
            }
            return null;
        }

        public Status Map(Data.Entities.Status status)
        {
            return Enum.Parse<Status>(status.ToString());
        }

        public Data.Entities.Status Map(Status status)
        {
            return Enum.Parse<Data.Entities.Status>(status.ToString());
        }

    }
}
