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

        public IActionResult OnGetGet()
        {
            return Process(
                new PageLoadModel { Name = "get" }
            );
        }

        public IActionResult OnGetMy()
        {
            return Process(
                new PageLoadModel { Name = "my" }
            );
        }

        public IActionResult OnGetInsights()
        {
            return Process(
                new PageLoadModel { Name = "insights" }
            );
        }

        public IActionResult OnGetOpen()
        {
            return Process(
                new PageLoadModel { Name = "open" }
            );
        }

        private IActionResult Process(PageLoadModel page)
        {
            TempData.Put("InitialPage", page);
            return Redirect("/");
        }

    }
}
