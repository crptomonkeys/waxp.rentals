using Nano.Net;
using Nano.Net.Numbers;

namespace WaxRentals.Banano.Config
{
    public static class Constants
    {

        public static class Protocol
        {
            public const string Prefix = "ban";
            public const string Address = "ban_1waxf5j83w7eqqz3kph7u843wh3p5ddf1n48rh4i9m41zhk9nnquzwbnz9pb";
            public const int Decimals = 29;
            public const decimal Minimum = 1;
            public static readonly BigDecimal MinimumTransaction = Amount.NanoToRaw(Minimum * 0.1m); // 1 BAN
        }

        public static class Locations
        {
            public const string Seed = "/run/secrets/banano.seed";
            public const string WelcomeSeed = "/run/secrets/welcome.banano.seed";
            public const string WorkServer = "http://work-server-01:7077";
#if DEBUG
            public const string Node = "http://host-x86-01:17072";
#else
            public const string Node = "http://banano_node:7072";
#endif
        }

    }
}
