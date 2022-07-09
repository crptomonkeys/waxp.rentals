using System.Net;
using Microsoft.AspNetCore.Mvc;
using WaxRentals.Api.Config;
using WaxRentals.Api.Entities;
using WaxRentals.Api.Entities.WelcomePackages;
using WaxRentals.Service.Shared.Connectors;

namespace WaxRentals.Api.Controllers
{
    public class WelcomePackagesController : ServiceBase
    {

        private IWelcomePackageService Packages { get; }
        private Mapper Mapper { get; }

        public WelcomePackagesController(ITrackService track, IWelcomePackageService packages, Mapper mapper)
            : base(track)
        {
            Packages = packages;
            Mapper = mapper;
        }

        [HttpPost("v1/Create")]
        [ProducesResponseType(typeof(Result<WelcomePackageInfo>), (int)HttpStatusCode.OK)]
        public async Task<JsonResult> Create([FromBody] string memo)
        {
            var result = await Packages.Create(memo);
            return result.Success
                ? await ByBananoAddress(result.Value.Address)
                : Fail<WelcomePackageInfo>(result.Error);
        }

        [HttpGet("v1/ByBananoAddress")]
        [ProducesResponseType(typeof(Result<WelcomePackageInfo>), (int)HttpStatusCode.OK)]
        public async Task<JsonResult> ByBananoAddress([FromBody] string address)
        {
            var result = await Packages.ByBananoAddress(address);
            return result.Success
                ? Succeed(Mapper.Map(result.Value))
                : Fail<WelcomePackageInfo>(result.Error);
        }

        [HttpPost("v1/ByBananoAddresses")]
        [ProducesResponseType(typeof(Result<IEnumerable<WelcomePackageInfo>>), (int)HttpStatusCode.OK)]
        public async Task<JsonResult> ByBananoAddresses([FromBody] IEnumerable<string> addresses)
        {
            var result = await Packages.ByBananoAddresses(addresses);
            return result.Success
                ? Succeed(result.Value.Select(Mapper.Map))
                : Fail<IEnumerable<WelcomePackageInfo>>(result.Error);
        }

    }
}