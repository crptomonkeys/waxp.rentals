using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WaxRentals.Service.Shared.Entities;

#nullable disable

namespace WaxRentals.Service.Shared.Connectors
{
    public interface IWelcomePackageService
    {
        Task<Result<NewWelcomePackage>> New(string memo);
        Task<Result<IEnumerable<WelcomePackageInfo>>> ByBananoAddresses(IEnumerable<string> addresses);
        Task<Result<WelcomePackageInfo>> ByBananoAddress(string address);
    }

    internal class WelcomePackageService : Connector, IWelcomePackageService
    {

        public WelcomePackageService(Uri baseUrl, ITrackService log) : base(baseUrl, log) { }

        public async Task<Result<NewWelcomePackage>> New(string memo)
        {
            return await Post<NewWelcomePackage>("New", memo);
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

    }
}
