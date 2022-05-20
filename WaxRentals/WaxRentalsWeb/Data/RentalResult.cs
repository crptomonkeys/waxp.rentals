namespace WaxRentalsWeb.Data
{
    public class RentalResult
    {

        public bool Success { get; set; }
        public string Error { get; set; }
        public string Address { get; set; }

        public static RentalResult Succeed(string address)
        {
            return new RentalResult { Success = true, Address = address };
        }

        public static RentalResult Fail(string error)
        {
            return new RentalResult { Success = false, Error = error };
        }

    }
}
