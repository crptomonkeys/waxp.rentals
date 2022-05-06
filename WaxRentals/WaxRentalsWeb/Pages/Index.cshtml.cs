using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using Eos.Api;
using Eos.Cryptography;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Logging;
using Nano.Net;
using Newtonsoft.Json.Linq;
using WaxRentals.Banano.Config;
using WaxRentals.Monitoring.Prices;
using WaxRentals.Waxp.Config;
using WaxRentals.Waxp.Transact;
//using WaxRentals.Banano.Transact;
using Banano = WaxRentals.Banano.Config.Constants;
using Waxp = WaxRentals.Waxp.Config.Constants;

namespace WaxRentalsWeb.Pages
{
    public class IndexModel : PageModel
    {
        private readonly ILogger<IndexModel> _logger;

        public IndexModel(ILogger<IndexModel> logger, PriceMonitor prices)
        {
            _logger = logger;
        }

        public async Task OnGet()
        {
            //var seed = JObject.Parse(
            //    System.IO.File.ReadAllText(Locations.Seed)
            //).ToObject<BananoSeed>();

            //var node = new RpcClient(Locations.Node);
            //var work = new RpcClient(Locations.WorkServer);

            //var account = new WrappedAccount(seed, 0, node, work);
            //var amount = await account.Receive();
            //await account.CheckRepresentative();
            //await account.Send(Protocol.Address, 1);

            //var key = JObject.Parse(
            //    System.IO.File.ReadAllText(Waxp.Locations.Key)
            //).ToObject<WaxKey>();
            //var active = new PrivateKey(key.Private);
            //var history = new NodeApiClient("https://history-wax-mainnet.wecan.dev");
            //var transact = new NodeApiClient("https://wax.eu.eosamsterdam.net");
            //var account = new WrappedAccount("rentwaxp4ban", active, history, transact);
            //var result = await account.Stake("rentwaxp4ban", 0.02M, 0.01M);
        }
    }
}
