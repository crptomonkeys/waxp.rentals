namespace WaxRentalsWeb.Data.Models
{
    public class BananoAddressModel
    {
        public string Full { get; }
        public string Start { get; }
        public string Mid { get; }
        public string End { get; }

        internal BananoAddressModel(string address)
        {
            Full = address;
            Start = address[..11];
            Mid = address[11..58];
            End = address[58..];
        }
    }
}
