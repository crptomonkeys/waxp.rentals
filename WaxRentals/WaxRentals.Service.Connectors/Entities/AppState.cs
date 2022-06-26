#nullable disable

namespace WaxRentals.Service.Shared.Entities
{
    public class AppState
    {

        public decimal BananoPrice { get; set; }
        public string BananoAddress { get; set; }
        public decimal BananoBalance { get; set; }
        
        public decimal WaxPrice { get; set; }
        public string WaxAccount { get; set; }
        public decimal WaxStaked { get; set; }
        public string WaxWorkingAccount { get; set; }
        public decimal WaxBalanceAvailableToday { get; set; }
        public decimal WaxBalanceAvailableTomorrow { get; set; }

    }
}
