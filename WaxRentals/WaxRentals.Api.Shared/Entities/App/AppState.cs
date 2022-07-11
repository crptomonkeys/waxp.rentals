#nullable disable

namespace WaxRentals.Api.Entities.App
{
    public class AppState
    {

        public BananoStateInfo Banano { get; set; }
        public WaxStateInfo Wax { get; set; }
        public CostsInfo Costs { get; set; }
        public LimitsInfo Limits { get; set; }
        public WelcomePackagesInfo WelcomePackages { get; set; }

    }
}
