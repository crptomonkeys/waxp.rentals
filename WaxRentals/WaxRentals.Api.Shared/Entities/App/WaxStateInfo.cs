#nullable disable

namespace WaxRentals.Api.Entities.App
{
    public class WaxStateInfo
    {

        public decimal Price { get; set; }
        public decimal Staked { get; set; }
        public string WorkingAccount { get; set; }
        public decimal AvailableToday { get; set; }
        public decimal AdditionalAvailableTomorrow { get; set; }

    }
}
