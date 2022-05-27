using Newtonsoft.Json;

namespace WaxRentals.Waxp.Transact
{
    public class Nft
    {

        [JsonProperty("asset_id")]
        public string AssetId { get; set; }

    }
}
