using System;

namespace WaxRentals.Monitoring.Config
{
    public static class Constants
    {

        public static readonly StringComparer Comparer = StringComparer.OrdinalIgnoreCase;
        public static readonly TimeSpan QuickTimeout = TimeSpan.FromSeconds(5);

        public static class Coins
        {
            public const string Banano = "banano";
            public const string Wax = "wax";
        }

        public static class Secrets
        {
            public const string TelegramInfo = "/run/secrets/telegram.waxp.rentals";
        }

    }
}
