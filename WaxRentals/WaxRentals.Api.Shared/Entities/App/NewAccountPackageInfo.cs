#nullable disable

namespace WaxRentals.Api.Entities.App
{
    public class NewAccountPackageInfo
    {

        public string WaxReceivingAccount { get; set; }
        public decimal WaxSend { get; set; }
        public string WaxMemoRegex { get; set; }
        public int FreeRentalCpu { get; set; }
        public int FreeRentalNet { get; set; }
        public int FreeRentalDays { get; set; }

    }
}
