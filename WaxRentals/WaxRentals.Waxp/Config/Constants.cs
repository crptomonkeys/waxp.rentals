﻿using static WaxRentals.Data.Config.Constants;

namespace WaxRentals.Waxp.Config
{
    public static class Constants
    {

        public static class Protocol
        {
            public const string Account = "rentwaxp4ban";
            public static readonly string[] TransactAccounts = new[] { "rentwax4ban1", "rentwax4ban2", "rentwax4ban3", "rentwax4ban4" };
            public const string HistoryBasePath = "/v2/history/get_actions?simple=true&sort=asc&limit=999&account=" + Account + "&after=";

            public const string TransactionEndpoints = "$..api_https2[*][1]";
            public const string HistoryEndpoints = "$..history_traditional_https[*][1]";
            public static readonly string[] EndpointsBlacklist = new[] { "https://api-wax.eosarabia.net" }; // Geo-blocked.

            public const string TransferBlocks = "$.simple_actions[?(@.action=='transfer' && @.data.to=='" + Account + "')]";

            public const decimal MinimumTransaction = Calculations.BananoPerWaxPerDay;

            public const string WaxAddressRegex = @"^[A-Za-z1-5\.]{1,12}$";
            public const string BananoAddressRegex = "^ban_[13]{1}[13456789abcdefghijkmnopqrstuwxyz]{59}$";

            public const int Decimals = 8;
            public static readonly string[] StakedWaxFormats = new string[]
            {
                "$.delegated_to[?(@.account_name!='{0}')].cpu_weight",
                "$.delegated_to[?(@.account_name!='{0}')].net_weight"
            };

            public const string NewUserAccount = "newuser.wax";
            public const decimal NewUserWax = 5;
            public const decimal NewUserCharge = 7;
            public const string NewUserMemoRegex = @"^[A-Za-z1-5\.]{1,14}DOTwam$";

            public const string Assets = "$.data[?(@.is_transferable==true)]";
        }

        public static class Locations
        {
            public const string Key = "/run/secrets/wax.key";
            public const string Endpoints = "https://validate.eosnation.io/wax/reports/endpoints.json";
            public const string StakedEndpointFormat = "https://lightapi.eosamsterdam.net/api/accinfo/wax/{0}";
            public const string Assets = "https://wax.api.atomicassets.io/atomicassets/v1/assets?owner=rentwaxp4ban";
        }

    }
}
