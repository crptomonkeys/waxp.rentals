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


        private IWaxAccounts Wax { get; }
        private ReaderWriterLockSlim Rwls { get; } = new();
        private IEnumerable<Nft> Nfts { get; set; } = Enumerable.Empty<Nft>();

        public NftsCache(IWaxAccounts wax, IDataFactory factory, TimeSpan interval)
            : base(factory, interval)
        {
            Wax = wax;
        }

        protected async override Task Tick()
        {
            try
            {
                var json = JObject.Parse(new QuickTimeoutWebClient().DownloadString(string.Format(Locations.Assets, Wax.Primary.Account), QuickTimeout));
                Rwls.SafeWrite(() =>
                    Nfts = json.SelectTokens(Protocol.Assets)
                               .Select(token => token.ToObject<Nft>())
                );
            }
            catch (Exception ex)
            {
                await Factory.Log.Error(ex, context: string.Format(Locations.Assets, Wax.Primary.Account));
            }
        }

    }
}
