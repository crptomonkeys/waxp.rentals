using Microsoft.AspNetCore.Mvc;
using WaxRentals.Api.Entities;
using WaxRentals.Service.Shared.Connectors;

namespace WaxRentals.Api.Controllers
{
    [ApiController]
    [Route("[controller]")]
    [Produces("application/json")]
    public abstract class ServiceBase : ControllerBase
    {

        protected ITrackService Track { get; }

        protected ServiceBase(ITrackService track)
        {
            Track = track;
        }

        protected JsonResult Json(object value)
        {
            return new JsonResult(value);
        }

        protected JsonResult Succeed<T>(T value)
        {
            return Json(Result<T>.Succeed(value));
        }

        protected JsonResult Fail<T>(string error)
        {
            return Json(Result<T>.Fail(error));
        }

    }
}
