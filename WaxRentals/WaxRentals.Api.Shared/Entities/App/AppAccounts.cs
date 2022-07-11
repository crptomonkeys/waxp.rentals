#nullable disable

namespace WaxRentals.Api.Entities.App
{
    public class AppAccounts
    {

        public string BananoSweepAddress { get; set; }
        public string WaxPrimaryAccount { get; set; }
        public IEnumerable<string> WaxTransactAccounts { get; set; }

    }
}
