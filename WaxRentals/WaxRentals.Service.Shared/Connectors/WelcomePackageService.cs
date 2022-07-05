using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WaxRentals.Service.Shared.Entities;
using WaxRentals.Service.Shared.Entities.Input;

#nullable disable

namespace WaxRentals.Service.Shared.Connectors
{
    public interface IWelcomePackageService
    {
        Task<Result<NewWelcomePackage>> Create(string memo);

        Task<Result<IEnumerable<WelcomePackageInfo>>> ByBananoAddresses(IEnumerable<string> addresses);
        Task<Result<WelcomePackageInfo>> ByBananoAddress(string address);

        Task<Result<IEnumerable<WelcomePackageInfo>>> New();
        Task<Result<IEnumerable<WelcomePackageInfo>>> Paid();
        Task<Result<IEnumerable<WelcomePackageInfo>>> MissingNfts();
        Task<Result<IEnumerable<WelcomePackageInfo>>> MissingRentals();
        Task<Result<IEnumerable<WelcomePackageInfo>>> Sweepable();

        Task<Result> ProcessPayment(int id);
        Task<Result> ProcessFunding(int id, string transaction);
        Task<Result> ProcessNft(int id, string transaction);
        Task<Result> ProcessRental(int id, int rentalId);
        Task<Result> ProcessSweep(int id, string transaction);
    }

    internal class WelcomePackageService : Connector, IWelcomePackageService
    {

        public WelcomePackageService(Uri baseUrl, ITrackService log) : base(baseUrl, log) { }

        public async Task<Result<NewWelcomePackage>> Create(string memo)
        {
            return await Post<NewWelcomePackage>("Create", memo);
        }

        public async Task<Result<IEnumerable<WelcomePackageInfo>>> ByBananoAddresses(IEnumerable<string> addresses)
        {
            return await Post<IEnumerable<WelcomePackageInfo>>("ByBananoAddresses", addresses);
        }

        public async Task<Result<WelcomePackageInfo>> ByBananoAddress(string address)
        {
            var response = await ByBananoAddresses(new string[] { address });
            return response.Success
                ? Result<WelcomePackageInfo>.Succeed(response.Value?.SingleOrDefault())
                : Result<WelcomePackageInfo>.Fail(response.Error);
        }

        public async Task<Result<IEnumerable<WelcomePackageInfo>>> New()
        {
            return await Get<IEnumerable<WelcomePackageInfo>>("New");
        }

        public async Task<Result<IEnumerable<WelcomePackageInfo>>> Paid()
        {
            return await Get<IEnumerable<WelcomePackageInfo>>("Paid");
        }

        public async Task<Result<IEnumerable<WelcomePackageInfo>>> MissingNfts()
        {
            return await Get<IEnumerable<WelcomePackageInfo>>("MissingNfts");
        }

        public async Task<Result<IEnumerable<WelcomePackageInfo>>> MissingRentals()
        {
            return await Get<IEnumerable<WelcomePackageInfo>>("MissingRentals");
        }

        public async Task<Result<IEnumerable<WelcomePackageInfo>>> Sweepable()
        {
            return await Get<IEnumerable<WelcomePackageInfo>>("Sweepable");
        }

        public async Task<Result> ProcessPayment(int id)
        {
            var input = new ProcessInput { Id = id };
            return await Post("ProcessPayment", input);
        }

        public async Task<Result> ProcessFunding(int id, string transaction)
        {
            var input = new ProcessInput { Id = id, Transaction = transaction };
            return await Post("ProcessFunding", input);
        }

        public async Task<Result> ProcessNft(int id, string transaction)
        {
            var input = new ProcessInput { Id = id, Transaction = transaction };
            return await Post("ProcessNft", input);
        }

        public async Task<Result> ProcessRental(int id, int rentalId)
        {
            var input = new ProcessWelcomePackageInput { Id = id, RentalId = rentalId };
            return await Post("ProcessRental", input);
        }

        public async Task<Result> ProcessSweep(int id, string transaction)
        {
            var input = new ProcessInput { Id = id, Transaction = transaction };
            return await Post("ProcessSweep", input);
        }

    }
}
