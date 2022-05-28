using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace WaxRentals.Data.Entities
{
	[Table("Address", Schema = "welcome")]
    public class WelcomeAddress
    {

		[Key]
		public int AddressId { get; set; }

		[Column("Address"), DatabaseGenerated(DatabaseGeneratedOption.Computed)]
		public string BananoAddress { get; set; }

	}
}
