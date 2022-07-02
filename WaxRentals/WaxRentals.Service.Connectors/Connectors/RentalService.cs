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
        Task<Result<string>> New(RentalInput input);
        Task<Result<IEnumerable<RentalInfo>>> ByWaxAccount(string account);
        Task<Result<IEnumerable<RentalInfo>>> ByBananoAddresses(IEnumerable<string> addresses);
        Task<Result<RentalInfo>> ByBananoAddress(string address);
    }

    internal class RentalService : Connector, IRentalService
    {

        public RentalService(Uri baseUrl, ITrackService log) : base(baseUrl, log) { }

        public async Task<Result<string>> New(RentalInput input)
        {
            return await Post<string>("New", input);
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

    }
}
