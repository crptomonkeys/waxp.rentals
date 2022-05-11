using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace WaxRentals.Data.Entities
{
    public class Credit
    {

		public int CreditId { get; set; }

		[DatabaseGenerated(DatabaseGeneratedOption.Computed)]
		public DateTime Inserted { get; set; }

		public decimal Banano { get; set; }
		public string BananoTransaction { get; set; }
		
		[Column("StatusId")]
		public Status Status { get; set; }

		public int AccountId { get; set; }
		public virtual Account Account { get; set; }

	}
}
