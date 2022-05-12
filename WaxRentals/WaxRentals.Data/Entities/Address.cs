using System.ComponentModel.DataAnnotations.Schema;

namespace WaxRentals.Data.Entities
{
    public class Address
    {
		
		public int AddressId { get; set; }

		[Column("Address"), DatabaseGenerated(DatabaseGeneratedOption.Computed)]
		public string BananoAddress { get; set; }

	}
}
