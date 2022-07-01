using System.ComponentModel.DataAnnotations;
using static WaxRentals.Service.Shared.Config.Constants.Wax;

namespace WaxRentalsWeb.Data
{
    public class RentalInput
    {

        [Required, RegularExpression(Protocol.AccountRegex)]
        public string Account { get; set; }

        [Required]
        public uint Days { get; set; }

        public uint CPU { get; set; }
        public uint NET { get; set; }

    }
}
