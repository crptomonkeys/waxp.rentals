namespace WaxRentals.Banano.Config
{
    public static class Constants
    {

        public static class Protocol
        {
            public const string Prefix = "ban";
            public const string Address = "ban_1waxf5j83w7eqqz3kph7u843wh3p5ddf1n48rh4i9m41zhk9nnquzwbnz9pb";
            public const int Decimals = 29;
        }

        public static class Locations
        {
            public const string Seed = "/run/secrets/banano.seed";
#if DEBUG
            public const string Node = "http://host-x86-01:17072";
            public const string WorkServer = "http://localhost:7076";
#else
            public const string Node = "http://node:7072";
            public const string WorkServer = "http://work-server:7076";
#endif
        }

    }
}
