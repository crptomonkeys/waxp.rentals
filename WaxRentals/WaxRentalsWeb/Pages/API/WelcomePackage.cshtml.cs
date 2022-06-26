using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using WaxRentals.Monitoring.Notifications;
using WaxRentals.Service.Shared.Connectors;
using WaxRentalsWeb.Data;
using WaxRentalsWeb.Data.Models;
using static WaxRentalsWeb.Data.WelcomePackageResult;

namespace WaxRentalsWeb.Pages
{
    public class WelcomePackageModel : PageModel
    {

        private ITelegramNotifier Telegram { get; }
        private IWelcomePackageService Service { get; }

        public WelcomePackageModel(ITelegramNotifier telegram, IWelcomePackageService service)
        {
            Telegram = telegram;
            Service = service;
        }

        public async Task<JsonResult> OnPostAsync(string memo)
        {
            var result = await Service.New(memo);
            if (result.Success)
            {
                Telegram.Send($"Starting welcome package process for {memo}.");
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
