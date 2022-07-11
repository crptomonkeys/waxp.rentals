#nullable disable

namespace WaxRentals.Api.Entities.Rentals
{
    public class NewRental
    {

        public string Account { get; set; }
        public int Days { get; set; }
        public decimal Cpu { get; set; }
        public decimal Net { get; set; }

    }
}
