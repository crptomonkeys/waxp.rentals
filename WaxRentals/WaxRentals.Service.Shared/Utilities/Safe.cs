namespace WaxRentals.Service.Shared.Utilities
{
    public static class Safe
    {

        public static decimal Divide(decimal numerator, decimal denominator)
        {
            if (numerator == 0 || denominator == 0)
            {
                return 0;
            }
            return numerator / denominator;
        }

    }
}
