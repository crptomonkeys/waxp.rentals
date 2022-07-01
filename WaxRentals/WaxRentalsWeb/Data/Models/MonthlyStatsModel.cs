using System.Globalization;
using WaxRentals.Service.Shared.Entities;

namespace WaxRentalsWeb.Data.Models
{
    public class MonthlyStatsModel
    {

        public int Year { get; }
        public string Month { get; }
        public decimal WaxDaysRented { get; }
        public decimal WaxDaysFree { get; }
        public decimal WaxPurchasedForSite { get; }
        public int WelcomePackagesOpened { get; }

        public MonthlyStatsModel(MonthlyStats stats)
        {
            Year                  = stats.Year;
            Month                 = CultureInfo.InvariantCulture.DateTimeFormat.GetAbbreviatedMonthName(stats.Month);
            WaxDaysRented         = stats.WaxDaysRented;
            WaxDaysFree           = stats.WaxDaysFree;
            WaxPurchasedForSite   = stats.WaxPurchasedForSite;
            WelcomePackagesOpened = stats.WelcomePackagesOpened;
        }

    }
}
