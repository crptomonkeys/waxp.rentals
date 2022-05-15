using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace WaxRentals.Data.Entities
{
	public class Rental
	{

		public int RentalId { get; set; }

		[DatabaseGenerated(DatabaseGeneratedOption.Computed)]
		public DateTime Inserted { get; set; }

		public string TargetWaxAccount { get; set; }
		public int RentalDays { get; set; }
		public decimal CPU { get; set; }
		public decimal NET { get; set; }

		public decimal Banano { get; set; }
		public DateTime? Paid { get; set; }

		[DatabaseGenerated(DatabaseGeneratedOption.Computed)]
		public DateTime? PaidThrough { get; set; }

		public string SourceWaxAccount { get; set; }
		public string StakeWaxTransaction { get; set; }
		public string UnstakeWaxTransaction { get; set; }

		[NotMapped]
		public Status Status {
			get { return (Status)StatusId; }
			set { StatusId = (int)value; }
		}
		public int StatusId { get; set; }

	}
}
