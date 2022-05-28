namespace WaxRentals.Processing.Tracking
{
    internal interface ITracker
    {

        void Track(string description, decimal quantity, string coin, decimal? earned = null, decimal? spent = null);

    }
}
