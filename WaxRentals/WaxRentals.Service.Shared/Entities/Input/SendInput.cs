#nullable disable

namespace WaxRentals.Service.Shared.Entities.Input
{
    public class SendBananoInput
    {

        public string Recipient { get; set; }
        public decimal Amount { get; set; }
        public string Reason { get; set; }

    }
}
