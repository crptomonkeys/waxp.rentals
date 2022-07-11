#nullable disable

namespace WaxRentals.Api.Entities.Rentals
{
    public class DatesInfo
    {

        public DateTime Paid { get; set; }
        public DateTime? Staked { get; set; }
        public DateTime? Expires { get; set; }

    }
}
