using System.ComponentModel.DataAnnotations;
using static WaxRentals.Waxp.Config.Constants;

namespace WaxRentalsWeb.Data
{
    public class RentalInput
    {

        [Required, RegularExpression(Protocol.WaxAddressRegex)]
        public string Account { get; set; }

        [Required]
        public uint Days { get; set; }

        public uint CPU { get; set; }
        public uint NET { get; set; }

    }
}
