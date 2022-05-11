using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace WaxRentals.Data.Entities
{
    public class Account
    {

		public int AccountId { get; set; }

		[DatabaseGenerated(DatabaseGeneratedOption.Computed)]
		public DateTime Inserted { get; set; }

		public string WaxAccount { get; set; }
		public decimal CPU { get; set; }
		public decimal NET { get; set; }
		public DateTime? PaidThrough { get; set; }

	}
}
