using System.Data.Entity;
using System.Data.Entity.ModelConfiguration.Conventions;
using WaxRentals.Data.Config;
using WaxRentals.Data.Entities;

namespace WaxRentals.Data.Context
{
    internal class WaxRentalsContext : DbContext
    {

        public DbSet<Address> Addresses { get; set; }
        public DbSet<Purchase> Purchases { get; set; }
        public DbSet<Rental> Rentals { get; set; }

        public DbSet<WaxHistory> WaxHistory { get; set; }

        public DbSet<Error> Errors { get; set; }
        public DbSet<Message> Messages { get; set; }

        #region " Setup "

        public WaxRentalsContext(WaxDb db) : base(db.ConnectionString) { }

        static WaxRentalsContext()
        {
            Database.SetInitializer<WaxRentalsContext>(null);
        }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            modelBuilder.Conventions.Remove<PluralizingTableNameConvention>();
        }

        #endregion

    }
}
