using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WaxRentals.Service.Shared.Entities;
using WaxRentals.Service.Shared.Entities.Input;

#nullable disable

namespace WaxRentals.Service.Shared.Connectors
{
    public interface IRentalService
    {
        Task<Result<NewRental>> Create(NewRentalInput input);

        Task<Result<IEnumerable<RentalInfo>>> ByWaxAccount(string account);
        Task<Result<IEnumerable<RentalInfo>>> ByBananoAddresses(IEnumerable<string> addresses);
        Task<Result<RentalInfo>> ByBananoAddress(string address);

        Task<Result<IEnumerable<RentalInfo>>> New();
        Task<Result<IEnumerable<RentalInfo>>> Paid();
        Task<Result<IEnumerable<RentalInfo>>> Sweepable();
        Task<Result<RentalInfo>> NextClosing();

        Task<Result> ProcessPayment(int id);
        Task<Result> ProcessStake(int id, string source, string transaction);
        Task<Result> ProcessSweep(int id, string transaction);
        Task<Result> ProcessClosing(int id, string transaction);

        Task<Result<RentalInfo>> Extend(string address, int days);
        Task<Result<RentalInfo>> Expire(string address);
    }

    internal class RentalService : Connector, IRentalService
    {

        public RentalService(Uri baseUrl, ITrackService log) : base(baseUrl, log) { }

        public async Task<Result<NewRental>> Create(NewRentalInput input)
        {
            return await Post<NewRental>("Create", input);
        }

        public async Task<Result<IEnumerable<RentalInfo>>> ByWaxAccount(string account)
        {
            return await Get<IEnumerable<RentalInfo>>($"ByWaxAccount/{account}");
        }

        public async Task<Result<IEnumerable<RentalInfo>>> ByBananoAddresses(IEnumerable<string> addresses)
        {
            return await Post<IEnumerable<RentalInfo>>("ByBananoAddresses", addresses);
        }

        public async Task<Result<RentalInfo>> ByBananoAddress(string address)
        {
            var response = await ByBananoAddresses(new string[] { address });
            return response.Success
                ? Result<RentalInfo>.Succeed(response.Value?.SingleOrDefault())
                : Result<RentalInfo>.Fail(response.Error);
        }

        public async Task<Result<IEnumerable<RentalInfo>>> New()
        {
            return await Get<IEnumerable<RentalInfo>>("New");
        }

        public async Task<Result<IEnumerable<RentalInfo>>> Paid()
        {
            return await Get<IEnumerable<RentalInfo>>("Paid");
        }

        public async Task<Result<IEnumerable<RentalInfo>>> Sweepable()
        {
            return await Get<IEnumerable<RentalInfo>>("Sweepable");
        }

        public async Task<Result<RentalInfo>> NextClosing()
        {
            return await Get<RentalInfo>("NextClosing");
        }

        public async Task<Result> ProcessPayment(int id)
        {
            var input = new ProcessInput { Id = id };
            return await Post("ProcessPayment", input);
        }

        public async Task<Result> ProcessStake(int id, string source, string transaction)
        {
            var input = new ProcessRentalInput { Id = id, Source = source, Transaction = transaction };
            return await Post("ProcessStake", input);
        }

        public async Task<Result> ProcessSweep(int id, string transaction)
        {
            var input = new ProcessInput { Id = id, Transaction = transaction };
            return await Post("ProcessSweep", input);
        }

        public async Task<Result> ProcessClosing(int id, string transaction)
        {
            var input = new ProcessInput { Id = id, Transaction = transaction };
            return await Post("ProcessClosing", input);
        }

        public async Task<Result<RentalInfo>> Extend(string address, int days)
        {
            var input = new ExtendRentalInput { Address = address, Days = days };
            return await Post<RentalInfo>("ExtendRental", input);
        }

        public async Task<Result<RentalInfo>> Expire(string address)
        {
            var input = new ExpireRentalInput { Address = address };
            return await Post<RentalInfo>("ExpireRental", input);
        }

    }
}
