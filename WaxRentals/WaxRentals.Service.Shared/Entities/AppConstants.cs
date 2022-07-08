#nullable disable

using System.Collections.Generic;

namespace WaxRentals.Service.Shared.Entities
{
    public class AppConstants
    {

        public string BananoSweepAddress { get; set; }
        public string WaxPrimaryAccount { get; set; }
        public IEnumerable<string> WaxTransactAccounts { get; set; }

    }
}
