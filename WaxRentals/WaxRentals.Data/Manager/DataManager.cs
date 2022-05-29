using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;
using WaxRentals.Data.Context;
using WaxRentals.Data.Entities;

namespace WaxRentals.Data.Manager
{
    internal class DataManager : IInsert, IProcess, ITrackWax, IWork, ILog, IExplore
    {

        private WaxRentalsContext Context { get; }
        private static DateTime Abandoned { get { return DateTime.UtcNow.AddDays(-1); } } // Abandon New Rentals after one day.

        public DataManager(WaxRentalsContext context)
        {
            Context = context;
        }

        #region " IInsert "

        public async Task<int> OpenRental(string account, int days, decimal cpu, decimal net, decimal banano)
        {
            // Prevent spamming of the same unpaid account info.
            var existing = Context.Rentals.SingleOrDefault(rental =>
                rental.TargetWaxAccount == account &&
                rental.RentalDays == days &&
                rental.CPU == cpu &&
                rental.NET == net &&
                rental.Banano == banano &&
                rental.StatusId == (int)Status.New &&
                rental.Inserted > Abandoned);
            if (existing != null)
            {
                return existing.RentalId;
            }

            var rental = Context.Rentals.Add(
                new Rental
                {
                    TargetWaxAccount = account,
                    RentalDays = days,
                    CPU = cpu,
                    NET = net,
                    Banano = banano,
                    Status = Status.New
                }
            );
            await Context.SaveChangesAsync();
            return rental.RentalId;
        }

        public async Task<bool> OpenPurchase(decimal wax, string transaction, string bananoAddress, decimal banano, Status status)
        {
            // Some endpoints don't filter very accurately, so make sure we're not trying to insert the same transaction more than once.
            // (Note that WaxTransaction has to be unique in the database, so this is just preventing an exception.)
            var exists = Context.Purchases.Any(purchase => purchase.WaxTransaction == transaction);
            if (!exists)
            {
                Context.Purchases.Add(
                    new Purchase
                    {
                        Wax = wax,
                        WaxTransaction = transaction,
                        PaymentBananoAddress = bananoAddress,
                        Banano = banano,
                        Status = status
                    }
                );
                await Context.SaveChangesAsync();
                return true;
            }
            return false;
        }

        public async Task<int> OpenWelcomePackage(string account, string memo, decimal wax, decimal banano)
        {
            // Prevent spamming of the same unpaid package info.
            var existing = Context.WelcomePackages.SingleOrDefault(package =>
                package.TargetWaxAccount == account &&
                package.Memo == memo &&
                package.Inserted > Abandoned);
            if (existing != null)
            {
                existing.Wax = wax;
                existing.Banano = banano;
                await Context.SaveChangesAsync();
                return existing.PackageId;
            }

            var package = Context.WelcomePackages.Add(
                new WelcomePackage
                {
                    TargetWaxAccount = account,
                    Memo = memo,
                    Wax = wax,
                    Banano = banano,
                    Status = Status.New
                }
            );
            await Context.SaveChangesAsync();
            return package.PackageId;
        }

        #endregion

        #region " IProcess "

        #region " Rentals "

        public Task<IEnumerable<Rental>> PullNewRentals()
        {
            // If the rental hasn't been funded within 24 hours, assume it's abandoned.
            // If it needs to be reactivated, change the Inserted date in the database.
            // But probably, the user will just make a new one.
            IEnumerable<Rental> rentals = Context.Rentals.Where(
                rental => rental.StatusId == (int)Status.New && rental.Inserted > Abandoned
            ).ToList();
            return Task.FromResult(rentals);
        }

        public async Task ProcessRentalPayment(int rentalId)
        {
            var rental = Context.Rentals.SingleOrDefault(rental => rental.RentalId == rentalId && rental.StatusId == (int)Status.New);
            if (rental != null)
            {
                rental.Paid = DateTime.UtcNow;
                rental.Status = Status.Pending;
                await Context.SaveChangesAsync();
            }
        }

        public Task<IEnumerable<Rental>> PullPaidRentalsToStake()
        {
            IEnumerable<Rental> rentals = Context.Rentals.Where(
                rental => rental.StatusId == (int)Status.Pending && rental.StakeWaxTransaction == null
            ).ToList();
            return Task.FromResult(rentals);
        }

        public async Task ProcessRentalStaking(int rentalId, string source, string transaction)
        {
            var rental = Context.Rentals.SingleOrDefault(rental => rental.RentalId == rentalId && rental.StatusId == (int)Status.Pending);
            if (rental != null)
            {
                rental.SourceWaxAccount = source;
                rental.StakeWaxTransaction = transaction;
                rental.Status = Status.Processed;
                await Context.SaveChangesAsync();
            }
        }

        public Task<IEnumerable<Rental>> PullSweepableRentals()
        {
            IEnumerable<Rental> rentals = Context.Rentals.Where(
                rental => rental.StatusId == (int)Status.Processed && rental.SweepBananoTransaction == null
            ).ToList();
            return Task.FromResult(rentals);
        }

        public async Task ProcessRentalSweep(int rentalId, string transaction)
        {
            var rental = Context.Rentals.Single(rental => rental.RentalId == rentalId && rental.StatusId == (int)Status.Processed);
            rental.SweepBananoTransaction = transaction;
            await Context.SaveChangesAsync();
        }

        public Task<Rental> PullNextClosingRental()
        {
            var rental = Context.Rentals.FirstOrDefault(rental =>
                rental.StatusId == (int)Status.Processed && rental.PaidThrough < DateTime.UtcNow);
            return Task.FromResult(rental);
        }

        public async Task ProcessRentalClosing(int rentalId, string transaction)
        {
            var rental = Context.Rentals.FirstOrDefault(rental =>
                rental.StatusId == (int)Status.Processed && rental.PaidThrough < DateTime.UtcNow);
            rental.UnstakeWaxTransaction = transaction;
            rental.Status = Status.Closed;
            await Context.SaveChangesAsync();
        }

        #endregion

        #region " Purchases "

        public async Task<Purchase> PullNextPurchase()
        {
            return await Context.Database
                                .SqlQuery<Purchase>("[dbo].[PullNextPurchase]")
                                .SingleOrDefaultAsync();
        }

        public async Task ProcessPurchase(int purchaseId, string transaction)
        {
            var purchase = Context.Purchases.Single(purchase => purchase.PurchaseId == purchaseId && purchase.StatusId == (int)Status.Pending);
            purchase.BananoTransaction = transaction;
            purchase.Status = Status.Processed;
            await Context.SaveChangesAsync();
        }

        #endregion

        #region " Welcome Packages "

        public async Task<IEnumerable<WelcomePackage>> PullNewWelcomePackages()
        {
            // If the package hasn't been funded within 24 hours, assume it's abandoned.
            // If it needs to be reactivated, change the Inserted date in the database.
            // But probably, the user will just make a new one.
            return await (from package in Context.WelcomePackages
                          where package.StatusId == (int)Status.New && package.Inserted > Abandoned
                          select package).ToArrayAsync();
        }

        public async Task ProcessWelcomePackagePayment(int packageId)
        {
            var package = Context.WelcomePackages.SingleOrDefault(package => package.PackageId == packageId && package.StatusId == (int)Status.New);
            if (package != null)
            {
                package.Paid = DateTime.UtcNow;
                package.Status = Status.Pending;
                await Context.SaveChangesAsync();
            }
        }

        public async Task<IEnumerable<WelcomePackage>> PullPaidWelcomePackagesToFund()
        {
            return await (from package in Context.WelcomePackages
                          where package.StatusId == (int)Status.Pending && package.FundTransaction == null
                          select package).ToArrayAsync();
        }

        public async Task ProcessWelcomePackageFunding(int packageId, string fundTransaction, string nftTransaction)
        {
            var package = Context.WelcomePackages.SingleOrDefault(package => package.PackageId == packageId && package.StatusId == (int)Status.Pending);
            if (package != null)
            {
                package.FundTransaction = fundTransaction;
                package.NftTransaction = nftTransaction;
                package.Status = Status.Processed;
                await Context.SaveChangesAsync();
            }
        }

        public async Task<IEnumerable<WelcomePackage>> PullSweepableWelcomePackages()
        {
            return await (from package in Context.WelcomePackages
                          where package.StatusId == (int)Status.Processed && package.SweepBananoTransaction == null
                          select package).ToArrayAsync();
        }

        public async Task ProcessWelcomePackageSweep(int packageId, string transaction)
        {
            var package = Context.WelcomePackages.SingleOrDefault(package => package.PackageId == packageId && package.StatusId == (int)Status.Processed);
            package.SweepBananoTransaction = transaction;
            await Context.SaveChangesAsync();
        }

        #endregion

        #endregion

        #region " ITrackWax "

        public DateTime? GetLastHistoryCheck()
        {
            return Context.WaxHistory
                          .OrderByDescending(history => history.LastRun)
                          .FirstOrDefault()
                         ?.LastRun;
        }

        public void SetLastHistoryCheck(DateTime last)
        {
            Context.WaxHistory.Add(new WaxHistory { LastRun = last });
            Context.SaveChanges();
        }

        #endregion

        #region " IWork "

        public Task<int?> PullNextAddress()
        {
            return Task.FromResult(
                Context.Addresses.FirstOrDefault(address => address.Work == null)?.AddressId
            );
        }

        public async Task SaveWork(int addressId, string work)
        {
            var address = Context.Addresses.SingleOrDefault(address => address.AddressId == addressId && address.Work == null);
            if (address != null)
            {
                address.Work = work;
                await Context.SaveChangesAsync();
            }
        }

        #endregion

        #region " ILog "

        public async Task Error(Exception exception, string error = null, object context = null)
        {
            try
            {
                var log = new Error
                {
                    Message = exception.Message,
                    ErrorObject = error,
                    StackTrace = exception.StackTrace,
                    TargetSite = exception.TargetSite?.Name,
                    InnerExceptions = exception.InnerException?.ToString()
                };
                if (context != null)
                {
                    log.Context = JToken.FromObject(context).ToString();
                }

                Context.Errors.Add(log);
                await Context.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
            }
        }

        public async Task Message(Guid requestId, string url, MessageDirection direction, string message)
        {
            try
            {
                var log = new Message
                {
                    RequestId = requestId,
                    Url = url,
                    Direction = Enum.GetName(direction),
                    MessageObject = message
                };

                Context.Messages.Add(log);
                await Context.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
            }
        }

        #endregion

        #region " IExplore "

        public IEnumerable<Rental> GetRecentRentals()
        {
            return Context.Rentals
                          .Where(rental => rental.RentalId > 5) // Filter out test transactions.
                          .Where(rental => rental.StatusId == (int)Status.Processed || rental.StatusId == (int)Status.Closed)
                          .OrderByDescending(rental => rental.RentalId)
                          .Take(10)
                          .ToArray();
        }

        public IEnumerable<Purchase> GetRecentPurchases()
        {
            return Context.Purchases
                          //.Where(purchase => purchase.PurchaseId > 14) // Filter out test transactions and early NFT transfers.
                          .Where(purchase => purchase.StatusId == (int)Status.Processed && purchase.BananoTransaction != null)
                          .OrderByDescending(purchase => purchase.PurchaseId)
                          .Take(10)
                          .ToArray();
        }

        public IEnumerable<WelcomePackage> GetRecentWelcomePackages()
        {
            return Context.WelcomePackages
                          .Where(package => package.PackageId > 1) // Filter out test transactions.
                          .Where(package => package.StatusId == (int)Status.Processed)
                          .OrderByDescending(package => package.PackageId)
                          .Take(10)
                          .ToArray();
        }

        public IEnumerable<Rental> GetRentalsByBananoAddresses(IEnumerable<string> addresses)
        {
            return from rental in Context.Rentals
                   join banano in Context.Addresses on rental.RentalId equals banano.AddressId
                   where addresses.Contains(banano.BananoAddress) && (rental.StatusId != (int)Status.New || rental.Inserted > Abandoned)
                   select rental;
        }

        public IEnumerable<Rental> GetRentalsByWaxAccount(string account)
        {
            return Context.Rentals.Where(rental =>
                rental.TargetWaxAccount == account && (rental.StatusId != (int)Status.New || rental.Inserted > Abandoned));
        }

        public IEnumerable<WelcomePackage> GetWelcomePackagesByBananoAddresses(IEnumerable<string> addresses)
        {
            return from package in Context.WelcomePackages
                   join banano in Context.WelcomeAddresses on package.PackageId equals banano.AddressId
                   where addresses.Contains(banano.BananoAddress) && (package.StatusId != (int)Status.New || package.Inserted > Abandoned)
                   select package;
        }

        #endregion

    }
}
