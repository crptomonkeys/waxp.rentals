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
            TempData.Put("InitialPage",
                new PageLoadModel
                {
                    Name = "bananoAddress",
                    Value = id
                }
            );
            return Redirect("/");
        }

        public IActionResult OnGetWA(string id)
        {
            TempData.Put("InitialPage",
                new PageLoadModel
                {
                    Name = "waxAccount",
                    Value = id
                }
            );
            return Redirect("/");
        }

    }
}
