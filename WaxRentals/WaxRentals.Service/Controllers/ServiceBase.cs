using Microsoft.AspNetCore.Mvc;
using WaxRentals.Data.Manager;
using WaxRentals.Service.Shared.Entities;

namespace WaxRentals.Service.Controllers
{
    [ApiController]
    [Route("[controller]")]
    [Produces("application/json")]
    public abstract class ServiceBase : ControllerBase
    {

        protected IDataFactory Factory { get; }

        protected ServiceBase(IDataFactory factory)
        {
            Factory = factory;
        }

        protected JsonResult Json(object value)
        {
            return new JsonResult(value);
        }

        protected JsonResult Succeed()
        {
            return Json(Result.Succeed());
        }

        protected JsonResult Succeed<T>(T value)
        {
            return Json(Result<T>.Succeed(value));
        }

        protected JsonResult Fail(string error)
        {
            return Json(Result.Fail(error));
        }

    }
}
