using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Logging;
using Nano.Net;
using Newtonsoft.Json.Linq;
using WaxRentals.Banano.Config;
using WaxRentals.Banano.Transact;
using static WaxRentals.Banano.Config.Constants;

namespace WaxRentalsWeb.Pages
{
    public class IndexModel : PageModel
    {
        private readonly ILogger<IndexModel> _logger;

        public IndexModel(ILogger<IndexModel> logger)
        {
            _logger = logger;
        }

        public async Task OnGet()
        {
            var seed = JObject.Parse(
                System.IO.File.ReadAllText(Locations.Seed)
            ).ToObject<BananoSeed>();

            var node = new RpcClient(Locations.Node);
            var work = new RpcClient(Locations.WorkServer);

            var account = new WrappedAccount(seed, 0, node, work);
            //var amount = await account.Receive();
            //await account.CheckRepresentative();
            await account.Send(Protocol.Address, 1);
        }
    }
}
