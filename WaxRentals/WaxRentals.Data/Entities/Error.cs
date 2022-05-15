using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace WaxRentals.Data.Entities
{
	[Table("Error", Schema = "logs")]
    public class Error
	{

		public int ErrorId { get; set; }

		[DatabaseGenerated(DatabaseGeneratedOption.Computed)]
		public DateTime Inserted { get; set; }

		public string Context { get; set; }

		public string Message { get; set; }
		public string StackTrace { get; set; }
		public string TargetSite { get; set; }
		public string InnerExceptions { get; set; }

	}
}
