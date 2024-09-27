#nullable disable

namespace WaxRentals.Service.Shared.Entities.Input
{
    public class SendNftInput
    {

        public string Recipient { get; set; }
        public string AssetId { get; set; }
        public string Memo { get; set; }
        public string Source { get; set; }

    }
}
