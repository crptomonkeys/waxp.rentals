using WaxRentalsWeb.Data.Models;

namespace WaxRentalsWeb.Data
{
    public class WelcomePackageResult
    {

        public bool Success { get; set; }
        public string Error { get; set; }
        public WelcomePackageDetail Detail { get; set; }

        public static WelcomePackageResult Succeed(WelcomePackageDetail detail)
        {
            return new WelcomePackageResult { Success = true, Detail = detail };
        }

        public static WelcomePackageResult Fail(string error)
        {
            return new WelcomePackageResult { Success = false, Error = error };
        }

        public class WelcomePackageDetail
        {
            public BananoAddressModel Address { get; set; }
            public string Link { get; set; }
            public string Account { get; set; }
            public string Memo { get; set; }
        }

    }
}
