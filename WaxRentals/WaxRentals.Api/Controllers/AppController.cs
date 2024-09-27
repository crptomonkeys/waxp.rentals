using System.Net;
using Microsoft.AspNetCore.Mvc;
using WaxRentals.Api.Config;
using WaxRentals.Api.Entities;
using WaxRentals.Api.Entities.App;
using WaxRentals.Service.Shared.Connectors;

namespace WaxRentals.Api.Controllers
{
    public class AppController : ServiceBase
    {

        private IAppService App { get; }
        private Mapper Mapper { get; }

        public AppController(ITrackService track, IAppService app, Mapper mapper)
            : base(track)
        {
            App = app;
            Mapper = mapper;
        }

        [HttpGet("v1/Constants")]
        [ProducesResponseType(typeof(Result<AppConstants>), (int)HttpStatusCode.OK)]
        public async Task<JsonResult> Constants()
        {
            var result = await App.Constants();
            return result.Success
                ? Succeed(Mapper.Map(result.Value))
                : Fail<AppConstants>(result.Error);
        }

        [HttpGet("v1/Insights")]
        [ProducesResponseType(typeof(Result<AppInsights>), (int)HttpStatusCode.OK)]
        public async Task<JsonResult> Insights()
        {
            var result = await App.Insights();
            return result.Success
                ? Succeed(Mapper.Map(result.Value))
                : Fail<AppInsights>(result.Error);
        }

        [HttpGet("v1/State")]
        [ProducesResponseType(typeof(Result<AppState>), (int)HttpStatusCode.OK)]
        public async Task<JsonResult> State()
        {
            var result = await App.State();
            return result.Success
                ? Succeed(Mapper.Map(result.Value))
                : Fail<AppState>(result.Error);
        }

    }
}