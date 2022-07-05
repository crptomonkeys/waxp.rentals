namespace WaxRentals.Waxp.Config
{
    public static class Constants
    {

        public static class Protocol
        {
            public const string HistoryBasePath = "/v2/history/get_actions?simple=true&sort=asc&limit=999&account={0}&after=";

            public const string TransactionEndpoints = "$..api_https2[*][1]";
            public const string HistoryEndpoints = "$..history_traditional_https[*][1]";
            public static readonly string[] EndpointsBlacklist = new[] { "https://api-wax.eosarabia.net" }; // Geo-blocked.

            public const string TransferBlocks = "$.simple_actions[?(@.action=='transfer' && @.data.to=='{0}' && @.data.amount>'0')]";

            public const decimal MinimumTransaction = 1;

            public const int Decimals = 8;
            public static readonly string[] StakedWaxFormats = new string[]
            {
                "$.delegated_to[?(@.account_name!='{0}')].cpu_weight",
                "$.delegated_to[?(@.account_name!='{0}')].net_weight"
            };

            public const string Assets = "$.data[?(@.is_transferable==true)]";
        }

        public static class Locations
        {
            public const string Endpoints = "https://validate.eosnation.io/wax/reports/endpoints.json";
            public const string StakedEndpointFormat = "https://lightapi.eosamsterdam.net/api/accinfo/wax/{0}";
            public const string Assets = "https://wax.api.atomicassets.io/atomicassets/v1/assets?owner={0}";
        }

    }
}
