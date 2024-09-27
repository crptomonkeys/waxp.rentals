using Microsoft.AspNetCore.Mvc;

namespace WaxRentals.Manage.Controllers
{
    [ApiController]
    [Route("[controller]")]
    [Produces("application/json")]
    public abstract class ServiceBase : ControllerBase
    {

        protected JsonResult Json(object value)
        {
            return new JsonResult(value);
        }

    }
}
