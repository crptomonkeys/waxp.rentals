using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace WaxRentals.Data.Entities
{
    public class Payment
    {

		public int PaymentId { get; set; }

		[DatabaseGenerated(DatabaseGeneratedOption.Computed)]
		public DateTime Inserted { get; set; }

		public string WaxAccount { get; set; }
		public decimal Wax { get; set; }
		public string WaxTransaction { get; set; }

		public string BananoAddress { get; set; }
		public decimal Banano { get; set; }
		public string BananoTransaction { get; set; }

		[NotMapped]
		public Status Status
		{
			get { return (Status)StatusId; }
			set { StatusId = (int)value; }
		}
		public int StatusId { get; set; }

	}
}
