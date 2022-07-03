using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using WaxRentals.Service.Shared.Connectors;
using WaxRentalsWeb.Config;
using WaxRentalsWeb.Data;
using WaxRentalsWeb.Data.Models;
using ServiceEntities = WaxRentals.Service.Shared.Entities;

namespace WaxRentalsWeb.Pages
{
    public class IndexModel : PageModel
    {

        public PageLoadModel InitialPage { get; private set; }

        private IRentalService Service { get; }
        private ITrackService Track { get; }

        public IndexModel(IRentalService service, ITrackService track)
        {
            Service = service;
            Track = track;
        }

        public void OnGet()
        {
            InitialPage = TempData.Get<PageLoadModel>("InitialPage") ?? new PageLoadModel { Name = "default" };
        }

        public async Task<JsonResult> OnPostAsync(RentalInput input)
        {
            var result = await Service.Create(Map(input));
            if (result.Success)
            {
                await Track.Notify($"Starting rental process for {input.Account}.");
                return Succeed(result.Value.Address);
            }
            else
            {
                return Fail(result.Error);
            }
        }

        private static ServiceEntities.Input.NewRentalInput Map(RentalInput input)
        {
            return new ServiceEntities.Input.NewRentalInput
            {
                Account = input.Account,
                Days = (int)input.Days,
                Cpu = input.CPU,
                Net = input.NET,
                Free = false
            };
        }

        private static JsonResult Succeed(string address) => new(RentalResult.Succeed(address));
        private static JsonResult Fail(string error) => new(RentalResult.Fail(error));

    }
}
