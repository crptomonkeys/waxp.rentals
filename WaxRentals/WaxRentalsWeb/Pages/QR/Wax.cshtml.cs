using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using WaxRentals.Service.Shared.Connectors;
using WaxRentalsWeb.Pages.QR;

namespace WaxRentalsWeb.Pages
{
    public class WaxModel : QRPageModel
    {

        private IAppService Service { get; }

        public WaxModel(IAppService service)
        {
            Service = service;
        }

        public async Task<IActionResult> OnGet()
        {
            var state = await Service.State();
            return GenerateQRCode(state.Value.WaxAccount);
        }

    }
}
