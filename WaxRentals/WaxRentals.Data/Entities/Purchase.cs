using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace WaxRentals.Data.Entities
{
    public class Purchase
    {

		public int PurchaseId { get; set; }

		[DatabaseGenerated(DatabaseGeneratedOption.Computed)]
		public DateTime Inserted { get; set; }

		public decimal Wax { get; set; }
		public string WaxTransaction { get; set; }

		public string PaymentBananoAddress { get; set; }
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
