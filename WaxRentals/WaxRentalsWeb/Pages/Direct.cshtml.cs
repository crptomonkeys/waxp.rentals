using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using WaxRentalsWeb.Config;
using WaxRentalsWeb.Data.Models;

namespace WaxRentalsWeb.Pages
{
    public class DirectModel : PageModel
    {

        public IActionResult OnGetBA(string id)
        {
            return Process(
                new PageLoadModel { Name = "bananoAddress", Value = id }
            );
        }

        public IActionResult OnGetWA(string id)
        {
            return Process(
                new PageLoadModel { Name = "waxAccount", Value = id }
            );
        }

        public IActionResult OnGetSell()
        {
            return Process(
                new PageLoadModel { Name = "sell" }
            );
        }

        private IActionResult Process(PageLoadModel page)
        {
            TempData.Put("InitialPage", page);
            return Redirect("/");
        }

    }
}
