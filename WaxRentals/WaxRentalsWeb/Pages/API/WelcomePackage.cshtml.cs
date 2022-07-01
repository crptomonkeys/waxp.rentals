using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using WaxRentals.Service.Shared.Connectors;
using WaxRentalsWeb.Data;
using WaxRentalsWeb.Data.Models;
using static WaxRentalsWeb.Data.WelcomePackageResult;

namespace WaxRentalsWeb.Pages
{
    public class WelcomePackageModel : PageModel
    {

        private IWelcomePackageService Service { get; }
        private ITrackService Track { get; }

        public WelcomePackageModel(IWelcomePackageService service, ITrackService track)
        {
            Service = service;
            Track = track;
        }

        public async Task<JsonResult> OnPostAsync(string memo)
        {
            var result = await Service.New(memo);
            if (result.Success)
            {
                await Track.Notify($"Starting welcome package process for {memo}.");
                var package = result.Value;
                return Succeed(new WelcomePackageDetail
                {
                    Address = new BananoAddressModel(package.Address),
                    Link = package.Link,
                    Account = package.Account,
                    Memo = package.Memo
                });
            }
            else
            {
                return Fail(result.Error);
            }
        }

        protected JsonResult Succeed(WelcomePackageDetail detail) => new JsonResult(WelcomePackageResult.Succeed(detail));
        protected JsonResult Fail(string error) => new JsonResult(WelcomePackageResult.Fail(error));

    }
}
