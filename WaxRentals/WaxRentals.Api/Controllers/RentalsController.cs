using System.Net;
using Microsoft.AspNetCore.Mvc;
using WaxRentals.Api.Config;
using WaxRentals.Api.Entities;
using WaxRentals.Api.Entities.Rentals;
using WaxRentals.Service.Shared.Connectors;

namespace WaxRentals.Api.Controllers
{
    public class RentalsController : ServiceBase
    {

        private IRentalService Rentals { get; }
        private Mapper Mapper { get; }

        public RentalsController(ITrackService track, IRentalService rentals, Mapper mapper)
            : base(track)
        {
            Rentals = rentals;
            Mapper = mapper;
        }

        [HttpPost("v1/Create")]
        [ProducesResponseType(typeof(Result<RentalInfo>), (int)HttpStatusCode.OK)]
        public async Task<JsonResult> Create([FromBody] NewRental rental)
        {
            var result = await Rentals.Create(Mapper.Map(rental));
            return result.Success
                ? await ByBananoAddress(result.Value.Address)
                : Fail<RentalInfo>(result.Error);
        }

        [HttpGet("v1/ByWaxAccount")]
        [ProducesResponseType(typeof(Result<IEnumerable<RentalInfo>>), (int)HttpStatusCode.OK)]
        public async Task<JsonResult> ByWaxAccount([FromBody] string account)
        {
            var result = await Rentals.ByWaxAccount(account);
            return result.Success
                ? Succeed(result.Value.Select(Mapper.Map))
                : Fail<IEnumerable<RentalInfo>>(result.Error);
        }

        [HttpGet("v1/ByBananoAddress")]
        [ProducesResponseType(typeof(Result<RentalInfo>), (int)HttpStatusCode.OK)]
        public async Task<JsonResult> ByBananoAddress([FromBody] string address)
        {
            var result = await Rentals.ByBananoAddress(address);
            return result.Success
                ? Succeed(Mapper.Map(result.Value))
                : Fail<RentalInfo>(result.Error);
        }

        [HttpPost("v1/ByBananoAddresses")]
        [ProducesResponseType(typeof(Result<IEnumerable<RentalInfo>>), (int)HttpStatusCode.OK)]
        public async Task<JsonResult> ByBananoAddresses([FromBody] IEnumerable<string> addresses)
        {
            var result = await Rentals.ByBananoAddresses(addresses);
            return result.Success
                ? Succeed(result.Value.Select(Mapper.Map))
                : Fail<IEnumerable<RentalInfo>>(result.Error);
        }

    }
}