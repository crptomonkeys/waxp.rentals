using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc.RazorPages;
using WaxRentals.Monitoring;

namespace WaxRentalsWeb.Pages
{
    public class HealthModel : PageModel
    {

        public HealthModel(IEnumerable<IGlobalMonitor> monitors)
        {
            // Just making sure that the global monitors are running.
        }

        public void OnGet()
        {
            
        }

    }
}
