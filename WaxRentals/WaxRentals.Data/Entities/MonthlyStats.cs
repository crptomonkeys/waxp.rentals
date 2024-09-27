#nullable disable

namespace WaxRentals.Data.Entities
{
    public class MonthlyStats
    {

        public int Year { get; set; }
        public int Month { get; set; }
        public decimal WaxDaysRented { get; set; }
        public decimal WaxDaysFree { get; set; }
        public decimal WaxPurchasedForSite { get; set; }
        public int WelcomePackagesOpened { get; set; }

    }
}
