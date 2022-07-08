#nullable disable

namespace WaxRentals.Service.Shared.Entities.Input
{
    public class NewRentalInput
    {

        public string Account { get; set; }
        public int Days { get; set; }
        public decimal Cpu { get; set; }
        public decimal Net { get; set; }
        public bool Free { get; set; }

    }
}
