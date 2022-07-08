using System;
using System.ComponentModel.DataAnnotations.Schema;

#nullable disable

namespace WaxRentals.Data.Entities
{
    [Table("WaxHistory", Schema = "tracking")]
    public class WaxHistory
    {

        public int WaxHistoryId { get; set; }

        [DatabaseGenerated(DatabaseGeneratedOption.Computed)]
        public DateTime Inserted { get; set; }

        public DateTime LastRun { get; set; }

    }
}
