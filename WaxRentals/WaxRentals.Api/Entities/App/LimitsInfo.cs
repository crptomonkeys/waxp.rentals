#nullable disable

namespace WaxRentals.Api.Entities.App
{
    public class LimitsInfo
    {

        public decimal BananoMinimumCredit { get; set; }
        public decimal WaxMinimumRent { get; set; }
        public decimal WaxMaximumRent { get; set; }
        public decimal WaxMinimumBuy { get; set; }
        public decimal WaxMaximumBuy { get; set; }

    }
}
