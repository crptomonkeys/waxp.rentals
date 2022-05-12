namespace WaxRentals.Waxp.Config
{
    public static class Constants
    {

        public static class Protocol
        {
            public const string Account = "rentwaxp4ban";
            public const string HistoryBasePath = "/v2/history/get_actions?simple=true&sort=asc&account=" + Account + "&after=";

            public const string TransactionEndpoints = "$..api_https2[*][1]";
            public const string HistoryEndpoints = "$..history_traditional_https[*][1]";

            public const string TransferBlocks = "$.simple_actions[?(@.action=='transfer' && @.data.to=='" + Account + "')].data";

            public const decimal MinimumTransaction = 1;

            public const string BananoAddressRegex = "^ban_[13]{1}[13456789abcdefghijkmnopqrstuwxyz]{59}$";
        }

        public static class Locations
        {
            public const string Key = "/run/secrets/wax.key";
            public const string Endpoints = "https://validate.eosnation.io/wax/reports/endpoints.json";
        }

    }
}
