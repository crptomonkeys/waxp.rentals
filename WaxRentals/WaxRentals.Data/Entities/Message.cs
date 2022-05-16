using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace WaxRentals.Data.Entities
{
	[Table("Message", Schema = "logs")]
    public class Message
	{

		public int MessageId { get; set; }

		[DatabaseGenerated(DatabaseGeneratedOption.Computed)]
		public DateTime Inserted { get; set; }

		public Guid RequestId { get; set; }

		public string Url { get; set; }
		public string Direction { get; set; }
		public string MessageObject { get; set; }

	}
}
