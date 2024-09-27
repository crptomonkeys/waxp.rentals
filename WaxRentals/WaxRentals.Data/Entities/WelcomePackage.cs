using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

#nullable disable

namespace WaxRentals.Data.Entities
{
	[Table("Packages", Schema = "welcome")]
    public class WelcomePackage
    {

		[Key]
		public int PackageId { get; set; }

		[DatabaseGenerated(DatabaseGeneratedOption.Computed)]
		public DateTime Inserted { get; set; }

		public string TargetWaxAccount { get; set; }
		public string Memo { get; set; }
		public decimal Wax { get; set; }

		public decimal Banano { get; set; }
		public string SweepBananoTransaction { get; set; }
		public DateTime? Paid { get; set; }

		public string FundTransaction { get; set; }
		public string NftTransaction { get; set; }

		public int? RentalId { get; set; }
		public virtual Rental Rental { get; set; }

		[NotMapped]
		public Status Status
		{
			get { return (Status)StatusId; }
			set { StatusId = (int)value; }
		}
		public int StatusId { get; set; }

		[Column("Address"), DatabaseGenerated(DatabaseGeneratedOption.Computed)]
		public string BananoAddress { get; set; }

	}
}
