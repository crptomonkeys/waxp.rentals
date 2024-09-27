using Newtonsoft.Json.Linq;
using WaxRentals.Data.Manager;
using WaxRentals.Monitoring.Extensions;
using WaxRentals.Waxp.Transact;
using static WaxRentals.Monitoring.Config.Constants;
using static WaxRentals.Waxp.Config.Constants;

#nullable disable

namespace WaxRentals.Service.Caching
{
    public class NftsCache : InvalidatableCache
    {

        public IEnumerable<Nft> GetNfts() => Rwls.SafeRead(() => Nfts);


        private IWaxAccounts Wax { get; }
        private HttpClient Client { get; }
        private ReaderWriterLockSlim Rwls { get; } = new();
        private IEnumerable<Nft> Nfts { get; set; } = Enumerable.Empty<Nft>();

        public NftsCache(IWaxAccounts wax, ILog log, TimeSpan interval)
            : base(log, interval)
        {
            Wax = wax;
            Client = new HttpClient { Timeout = QuickTimeout };
        }

        protected async override Task Tick()
        {
            try
            {
                var random = new Random();
                var data = await Client.GetStringAsync(string.Format(Locations.Assets, Wax.Primary.Account));
                var json = JObject.Parse(data);
                Rwls.SafeWrite(() =>
                    Nfts = json.SelectTokens(Protocol.Assets)
                               .Select(token => token.ToObject<Nft>())
                               .OrderBy(nft => random.Next()) // Randomize for better distribution distribution.
                );
            }
            catch (Exception ex)
            {
                await Log.Error(ex, context: string.Format(Locations.Assets, Wax.Primary.Account));
            }
        }

    }
}
