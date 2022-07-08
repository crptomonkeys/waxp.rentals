#nullable disable

namespace WaxRentals.Service.Shared.Entities
{
    public class PurchaseInfo
    {

        public int Id { get; set; }
        public decimal Wax { get; set; }
        public string WaxTransaction { get; set; }
        public decimal Banano { get; set; }
        public string BananoAddress { get; set; }
        public string BananoTransaction { get; set; }

    }
}
