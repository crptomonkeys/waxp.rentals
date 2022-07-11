namespace WaxRentalsWeb.Config
{
    public class ApiContext
    {

        private string BaseUrl { get; }

        public ApiContext(string baseUrl)
        {
            BaseUrl = baseUrl;
        }

        public string AppConstants => $"{BaseUrl}/App/v1/Constants";
        public string AppInsights => $"{BaseUrl}/App/v1/Insights";
        public string AppState => $"{BaseUrl}/App/v1/State";

        public string CreateRental => $"{BaseUrl}/Rentals/v1/Create";
        public string RentalByBananoAddress => $"{BaseUrl}/Rentals/v1/ByBananoAddress";
        public string RentalsByBananoAddresses => $"{BaseUrl}/Rentals/v1/ByBananoAddresses";
        public string RentalsByWaxAccount => $"{BaseUrl}/Rentals/v1/ByWaxAccount";

        public string CreateWelcomePackage => $"{BaseUrl}/WelcomePackages/v1/Create";
        public string WelcomePackageByBananoAddress => $"{BaseUrl}/WelcomePackages/v1/ByBananoAddress";
        public string WelcomePackagesByBananoAddresses => $"{BaseUrl}/WelcomePackages/v1/ByBananoAddresses";

    }
}
