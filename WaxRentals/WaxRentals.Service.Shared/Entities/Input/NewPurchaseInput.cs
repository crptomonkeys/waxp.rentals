#nullable disable

namespace WaxRentals.Service.Shared.Entities.Input
{
    public class NewPurchaseInput
    {

        public decimal Amount { get; set; }
        public string Transaction { get; set; }
        public string BananoPaymentAddress { get; set; }
        public decimal Banano { get; set; }
        public Status Status { get; set; }

    }
}
