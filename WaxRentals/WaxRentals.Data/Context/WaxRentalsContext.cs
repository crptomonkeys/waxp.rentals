using System.Data.Entity;
using System.Data.Entity.ModelConfiguration.Conventions;
using WaxRentals.Data.Entities;

namespace WaxRentals.Data.Context
{
    internal class WaxRentalsContext : DbContext
    {

        public DbSet<Account> Accounts { get; set; }
        public DbSet<Address> Addresses { get; set; }
        public DbSet<Credit> Credits { get; set; }
        public DbSet<Payment> Payments { get; set; }

        #region " Setup "

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
