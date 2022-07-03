using static WaxRentals.Service.Shared.Config.Constants;

namespace WaxRentals.Service.Config
{
    public static class Constants
    {

        public static class Calculations
        {
            public const decimal BananoPerWaxPerDay = 1;
            public const int DaysDoubleThreshold = 15;
        }

        public static class Http
        {
            public static readonly TimeSpan QuickTimeout = TimeSpan.FromSeconds(5);
        }

        public static class Locations
        {
            public const string CoinPrices =
                $"https://api.coingecko.com/api/v3/simple/price?vs_currencies=usd&include_24hr_change=true&ids={Coins.Banano},{Coins.Wax}";
        }

    }
}
