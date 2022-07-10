using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.RazorPages;
using WaxRentals.Api.Entities.App;
using WaxRentalsWeb.Config;
using WaxRentalsWeb.Data.Models;
using WaxRentalsWeb.Net;

namespace WaxRentalsWeb.Pages
{
    public class IndexModel : PageModel
    {

        public ApiProxy Proxy { get; }
        public ApiContext Endpoints => Proxy.Endpoints;
        public AppConstants Constants { get; private set; }
        public PageLoadModel InitialPage { get; private set; }

        public IndexModel(ApiProxy proxy)
        {
            Proxy = proxy;
        }

        public async Task OnGet()
        {
            Constants = (await Proxy.Get<AppConstants>(Proxy.Endpoints.AppConstants)).Value;
            InitialPage = TempData.Get<PageLoadModel>("InitialPage") ?? new PageLoadModel { Name = "default" };
        }

    }
}
