namespace WaxRentals.Service.Shared.Config
{
    public static class Constants
    {
        public static class Coins
        {
            public const string Banano = "banano";
            public const string Wax = "wax";
        }

        public static class Banano
        {
            public static class Protocol
            {
                public const string AddressRegex = "^ban_[13]{1}[13456789abcdefghijkmnopqrstuwxyz]{59}$";
            }
        }

        public static class Wax
        {
            public static class NewUser
            {
                public const string Account = "newuser.wax";
                public const decimal OpenWax = 5;
                public const decimal ChargeWax = 7;
                public const string MemoRegex = @"^([a-z1-5.]|DOT){1,8}(\.wam|DOTwam)$";
                public const string MemoRefundOnExists = ":refund_on_exists";
                public const int FreeRentalDays = 1;
                public const int FreeCpu = 10;
                public const int FreeNet = 1;
            }

            public static class Protocol
            {
                public const string AccountRegex = @"^[A-Za-z1-5\.]{1,12}$";
            }
        }
    }
}
