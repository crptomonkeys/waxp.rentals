#nullable disable

namespace WaxRentals.Service.Shared.Entities.Input
{
    public class SendWaxInput
    {

        public string Recipient { get; set; }
        public decimal Amount { get; set; }
        public string Source { get; set; }
        public string Memo { get; set; }

    }
}
