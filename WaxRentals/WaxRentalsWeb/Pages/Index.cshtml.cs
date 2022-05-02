using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json.Linq;
using WaxRentals.Banano.Config;
using WaxRentals.Banano.Transact;

namespace WaxRentalsWeb.Pages
{
    public class IndexModel : PageModel
    {
        private readonly ILogger<IndexModel> _logger;

        public IndexModel(ILogger<IndexModel> logger)
        {
            _logger = logger;
        }

        public async void OnGet()
        {
            var seed = JObject.Parse(
                System.IO.File.ReadAllText("/run/secrets/banano.seed")
            ).ToObject<BananoSeed>();
            var account = new WrappedAccount(seed, 0);
            var amount = await account.Receive();
        }
    }
}
