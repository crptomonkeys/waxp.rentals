#nullable disable

namespace WaxRentals.Service.Shared.Entities.Input
{
    public class TransactionLog
    {

        public string Description { get; set; }
        public decimal Quantity { get; set; }
        public string Coin { get; set; }
        public decimal? Earned { get; set; }
        public decimal? Spent { get; set; }

    }
}
