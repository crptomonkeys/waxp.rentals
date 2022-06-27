using Newtonsoft.Json.Linq;
using WaxRentals.Data.Manager;
using WaxRentals.Monitoring.Extensions;
using WaxRentals.Waxp;
using WaxRentals.Waxp.Transact;
using static WaxRentals.Monitoring.Config.Constants;
using static WaxRentals.Waxp.Config.Constants;

#nullable disable

namespace WaxRentals.Service.Caching
{
    public class NftsCache : InvalidatableCache
    {

        public IEnumerable<Nft> GetNfts() => Rwls.SafeRead(() => Nfts);


        private ReaderWriterLockSlim Rwls { get; } = new();
        private IEnumerable<Nft> Nfts { get; set; } = Enumerable.Empty<Nft>();

        public NftsCache(IDataFactory factory, TimeSpan interval)
            : base(factory, interval)
        {
            
        }

        protected async override Task Tick()
        {
            try
            {
                var json = JObject.Parse(new QuickTimeoutWebClient().DownloadString(Locations.Assets, QuickTimeout));
                Rwls.SafeWrite(() =>
                    Nfts = json.SelectTokens(Protocol.Assets)
                               .Select(token => token.ToObject<Nft>())
                );
            }
            catch (Exception ex)
            {
                await Factory.Log.Error(ex, context: Locations.Assets);
            }
        }

    }
}
