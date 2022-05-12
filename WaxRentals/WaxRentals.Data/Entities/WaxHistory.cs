using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace WaxRentals.Data.Entities
{
    public class WaxHistory
    {

        public int WaxHistoryId { get; set; }

        [DatabaseGenerated(DatabaseGeneratedOption.Computed)]
        public DateTime Inserted { get; set; }

        public DateTime LastRun { get; set; }

    }
}
